using Microsoft.EntityFrameworkCore;
using OmniBus.Application.DTOs;
using OmniBus.Application.Interfaces;
using OmniBus.Domain.Enums;
using OmniBus.Infrastructure.Persistence;

namespace OmniBus.Infrastructure.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly ApplicationDbContext _context;

    public AnalyticsService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AnalyticsDto> GetDashboardAnalyticsAsync()
    {
        var analytics = new AnalyticsDto
        {
            Revenue = await GetRevenueStatsAsync(),
            DailyBookings = await GetDailyBookingsAsync(30),
            RoutePopularity = await GetRoutePopularityAsync(10),
            OccupancyRates = await GetOccupancyRatesAsync(DateTime.Today),
            SystemStats = await GetSystemStatsAsync()
        };

        return analytics;
    }

    public async Task<RevenueStatsDto> GetRevenueStatsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        endDate ??= DateTime.Today;
        startDate ??= endDate.Value.AddMonths(-6);

        var payments = await _context.Payments
            .Where(p => p.Status == PaymentStatus.Completed && 
                       p.CreatedAt >= startDate && 
                       p.CreatedAt <= endDate)
            .ToListAsync();

        var totalRevenue = payments.Sum(p => p.Amount);
        var monthlyRevenue = payments
            .Where(p => p.CreatedAt >= DateTime.Today.AddMonths(-1))
            .Sum(p => p.Amount);
        var dailyRevenue = payments
            .Where(p => p.CreatedAt >= DateTime.Today)
            .Sum(p => p.Amount);

        // Calculate growth rate
        var previousMonthRevenue = payments
            .Where(p => p.CreatedAt >= DateTime.Today.AddMonths(-2) && 
                       p.CreatedAt < DateTime.Today.AddMonths(-1))
            .Sum(p => p.Amount);
        var growthRate = previousMonthRevenue > 0 
            ? ((monthlyRevenue - previousMonthRevenue) / previousMonthRevenue) * 100 
            : 0;

        // Group by month
        var monthlyData = payments
            .GroupBy(p => new { p.CreatedAt.Year, p.CreatedAt.Month })
            .Select(g => new MonthlyRevenueDto
            {
                Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                Revenue = g.Sum(p => p.Amount),
                Bookings = g.Count()
            })
            .OrderBy(m => m.Month)
            .ToList();

        return new RevenueStatsDto
        {
            TotalRevenue = totalRevenue,
            MonthlyRevenue = monthlyRevenue,
            DailyRevenue = dailyRevenue,
            GrowthRate = growthRate,
            MonthlyData = monthlyData
        };
    }

    public async Task<List<DailyBookingDto>> GetDailyBookingsAsync(int days = 30)
    {
        var startDate = DateTime.Today.AddDays(-days);

        var tickets = await _context.Tickets
            .Where(t => t.BookingDate >= startDate)
            .ToListAsync();

        var dailyBookings = tickets
            .GroupBy(t => t.BookingDate.Date)
            .Select(g => new DailyBookingDto
            {
                Date = g.Key.ToString("yyyy-MM-dd"),
                Bookings = g.Count(t => t.Status != TicketStatus.Cancelled),
                Cancellations = g.Count(t => t.Status == TicketStatus.Cancelled)
            })
            .OrderBy(d => d.Date)
            .ToList();

        // Fill in missing dates with zero bookings
        var allDates = Enumerable.Range(0, days)
            .Select(i => DateTime.Today.AddDays(-days + i))
            .Select(d => d.ToString("yyyy-MM-dd"))
            .ToList();

        var result = allDates.Select(date =>
        {
            var existing = dailyBookings.FirstOrDefault(d => d.Date == date);
            return existing ?? new DailyBookingDto { Date = date, Bookings = 0, Cancellations = 0 };
        }).ToList();

        return result;
    }

    public async Task<List<RoutePopularityDto>> GetRoutePopularityAsync(int topN = 10)
    {
        var routeStats = await _context.Tickets
            .Include(t => t.Schedule)
                .ThenInclude(s => s.Route)
            .Include(t => t.Schedule)
                .ThenInclude(s => s.Bus)
            .Where(t => t.Status != TicketStatus.Cancelled)
            .GroupBy(t => t.Schedule.Route)
            .Select(g => new
            {
                Route = g.Key,
                TotalBookings = g.Count(),
                Revenue = g.Sum(t => t.Price),
                TotalSeats = g.Sum(t => t.Schedule.Bus.Capacity),
                BookedSeats = g.Count()
            })
            .OrderByDescending(r => r.TotalBookings)
            .Take(topN)
            .ToListAsync();

        var result = routeStats.Select(r => new RoutePopularityDto
        {
            RouteId = r.Route.Id,
            RouteName = $"{r.Route.Origin} → {r.Route.Destination}",
            TotalBookings = r.TotalBookings,
            Revenue = r.Revenue,
            AverageOccupancy = r.TotalSeats > 0 ? (double)r.BookedSeats / r.TotalSeats * 100 : 0
        }).ToList();

        return result;
    }

    public async Task<List<OccupancyRateDto>> GetOccupancyRatesAsync(DateTime? date = null)
    {
        date ??= DateTime.Today;
        var nextDay = date.Value.AddDays(1);

        var schedules = await _context.Schedules
            .Include(s => s.Route)
            .Include(s => s.Bus)
            .Include(s => s.Tickets)
            .Where(s => s.DepartureTime >= date && s.DepartureTime < nextDay)
            .ToListAsync();

        var result = schedules.Select(s => new OccupancyRateDto
        {
            ScheduleId = s.Id,
            RouteName = $"{s.Route.Origin} → {s.Route.Destination}",
            DepartureTime = s.DepartureTime,
            TotalSeats = s.Bus.Capacity,
            BookedSeats = s.Tickets.Count(t => t.Status != TicketStatus.Cancelled),
            OccupancyPercentage = s.Bus.Capacity > 0 
                ? (double)s.Tickets.Count(t => t.Status != TicketStatus.Cancelled) / s.Bus.Capacity * 100 
                : 0
        }).OrderByDescending(o => o.OccupancyPercentage).ToList();

        return result;
    }

    public async Task<SystemStatsDto> GetSystemStatsAsync()
    {
        var stats = new SystemStatsDto
        {
            TotalUsers = await _context.Users.CountAsync(u => u.Role == UserRole.Passenger),
            TotalDrivers = await _context.Users.CountAsync(u => u.Role == UserRole.Driver),
            TotalBuses = await _context.Buses.CountAsync(),
            TotalRoutes = await _context.Routes.CountAsync(),
            ActiveSchedules = await _context.Schedules
                .CountAsync(s => s.DepartureTime >= DateTime.Today),
            PendingPayments = await _context.Payments
                .CountAsync(p => p.Status == PaymentStatus.Pending)
        };

        return stats;
    }
}
