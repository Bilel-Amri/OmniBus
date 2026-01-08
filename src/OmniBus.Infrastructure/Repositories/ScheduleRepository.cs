using Microsoft.EntityFrameworkCore;
using OmniBus.Domain.Entities;
using OmniBus.Domain.Enums;
using OmniBus.Domain.Interfaces;
using OmniBus.Infrastructure.Persistence;

namespace OmniBus.Infrastructure.Repositories;

/// <summary>
/// Schedule repository implementation
/// </summary>
public class ScheduleRepository : IScheduleRepository
{
    private readonly ApplicationDbContext _context;
    
    public ScheduleRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Schedule?> GetByIdAsync(Guid id)
    {
        return await _context.Schedules
            .Include(s => s.Bus)
            .Include(s => s.Route)
            .Include(s => s.Driver)
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
    }
    
    public async Task<IEnumerable<Schedule>> GetAllAsync()
    {
        return await _context.Schedules
            .Include(s => s.Bus)
            .Include(s => s.Route)
            .Include(s => s.Driver)
            .Where(s => !s.IsDeleted)
            .OrderBy(s => s.DepartureTime)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Schedule>> GetByRouteAsync(Guid routeId)
    {
        return await _context.Schedules
            .Include(s => s.Bus)
            .Include(s => s.Route)
            .Include(s => s.Driver)
            .Where(s => s.RouteId == routeId && !s.IsDeleted)
            .OrderBy(s => s.DepartureTime)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Schedule>> GetByBusAsync(Guid busId)
    {
        return await _context.Schedules
            .Include(s => s.Bus)
            .Include(s => s.Route)
            .Include(s => s.Driver)
            .Where(s => s.BusId == busId && !s.IsDeleted)
            .OrderBy(s => s.DepartureTime)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Schedule>> GetByDriverAsync(Guid driverId)
    {
        return await _context.Schedules
            .Include(s => s.Bus)
            .Include(s => s.Route)
            .Where(s => s.DriverId == driverId && !s.IsDeleted)
            .OrderBy(s => s.DepartureTime)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Schedule>> GetUpcomingSchedulesAsync(DateTime fromDate)
    {
        return await _context.Schedules
            .Include(s => s.Bus)
            .Include(s => s.Route)
            .Include(s => s.Driver)
            .Where(s => s.DepartureTime >= fromDate && !s.IsDeleted && s.Status != ScheduleStatus.Cancelled)
            .OrderBy(s => s.DepartureTime)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Schedule>> GetSchedulesByDateAsync(DateTime date)
    {
        var startOfDay = date.Date;
        var endOfDay = date.Date.AddDays(1);
        
        return await _context.Schedules
            .Include(s => s.Bus)
            .Include(s => s.Route)
            .Include(s => s.Driver)
            .Where(s => s.DepartureTime >= startOfDay && s.DepartureTime < endOfDay && !s.IsDeleted)
            .OrderBy(s => s.DepartureTime)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Schedule>> SearchSchedulesAsync(string origin, string destination, DateTime date)
    {
        var startOfDay = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
        var endOfDay = DateTime.SpecifyKind(date.Date.AddDays(1), DateTimeKind.Utc);
        
        return await _context.Schedules
            .Include(s => s.Bus)
            .Include(s => s.Route)
            .Include(s => s.Driver)
            .Where(s => s.Route.Origin.ToLower().Contains(origin.ToLower()) &&
                   s.Route.Destination.ToLower().Contains(destination.ToLower()) &&
                   s.DepartureTime >= startOfDay && s.DepartureTime < endOfDay &&
                   s.AvailableSeats > 0 && !s.IsDeleted && s.Status != ScheduleStatus.Cancelled)
            .OrderBy(s => s.DepartureTime)
            .ToListAsync();
    }
    
    public async Task<Schedule> CreateAsync(Schedule schedule)
    {
        schedule.CreatedAt = DateTime.UtcNow;
        schedule.AvailableSeats = schedule.Bus.Capacity;
        _context.Schedules.Add(schedule);
        await _context.SaveChangesAsync();
        return schedule;
    }
    
    public async Task<Schedule> UpdateAsync(Schedule schedule)
    {
        schedule.UpdatedAt = DateTime.UtcNow;
        _context.Schedules.Update(schedule);
        await _context.SaveChangesAsync();
        return schedule;
    }
    
    public async Task<bool> DeleteAsync(Guid id)
    {
        var schedule = await GetByIdAsync(id);
        if (schedule == null) return false;
        
        schedule.IsDeleted = true;
        schedule.UpdatedAt = DateTime.UtcNow;
        _context.Schedules.Update(schedule);
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Schedules
            .AnyAsync(s => s.Id == id && !s.IsDeleted);
    }
    
    public async Task UpdateScheduleLocationAsync(Guid scheduleId, double latitude, double longitude)
    {
        var schedule = await GetByIdAsync(scheduleId);
        if (schedule != null)
        {
            schedule.CurrentLatitude = latitude;
            schedule.CurrentLongitude = longitude;
            schedule.LastTrackingUpdate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task DecrementAvailableSeatsAsync(Guid scheduleId, int count = 1)
    {
        var schedule = await GetByIdAsync(scheduleId);
        if (schedule != null)
        {
            schedule.AvailableSeats = Math.Max(0, schedule.AvailableSeats - count);
            _context.Schedules.Update(schedule);
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task IncrementAvailableSeatsAsync(Guid scheduleId, int count = 1)
    {
        var schedule = await GetByIdAsync(scheduleId);
        if (schedule != null)
        {
            schedule.AvailableSeats = Math.Min(schedule.Bus.Capacity, schedule.AvailableSeats + count);
            _context.Schedules.Update(schedule);
            await _context.SaveChangesAsync();
        }
    }
}

/// <summary>
/// Ticket repository implementation
/// </summary>
public class TicketRepository : ITicketRepository
{
    private readonly ApplicationDbContext _context;
    
    public TicketRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Ticket?> GetByIdAsync(Guid id)
    {
        return await _context.Tickets
            .Include(t => t.User)
            .Include(t => t.Schedule)
                .ThenInclude(s => s.Bus)
            .Include(t => t.Schedule)
                .ThenInclude(s => s.Route)
            .Include(t => t.Payment)
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
    }
    
    public async Task<Ticket?> GetByBookingReferenceAsync(string reference)
    {
        return await _context.Tickets
            .Include(t => t.User)
            .Include(t => t.Schedule)
                .ThenInclude(s => s.Bus)
            .Include(t => t.Schedule)
                .ThenInclude(s => s.Route)
            .Include(t => t.Payment)
            .FirstOrDefaultAsync(t => t.BookingReference == reference && !t.IsDeleted);
    }
    
    public async Task<IEnumerable<Ticket>> GetAllAsync()
    {
        return await _context.Tickets
            .Include(t => t.User)
            .Include(t => t.Schedule)
            .Where(t => !t.IsDeleted)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Ticket>> GetByUserAsync(Guid userId)
    {
        return await _context.Tickets
            .Include(t => t.Schedule)
                .ThenInclude(s => s.Bus)
            .Include(t => t.Schedule)
                .ThenInclude(s => s.Route)
            .Where(t => t.UserId == userId && !t.IsDeleted)
            .OrderByDescending(t => t.Schedule.DepartureTime)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Ticket>> GetByScheduleAsync(Guid scheduleId)
    {
        return await _context.Tickets
            .Include(t => t.User)
            .Where(t => t.ScheduleId == scheduleId && !t.IsDeleted)
            .OrderBy(t => t.SeatNumber)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Ticket>> GetUpcomingByUserAsync(Guid userId)
    {
        var now = DateTime.UtcNow;
        return await _context.Tickets
            .Include(t => t.Schedule)
                .ThenInclude(s => s.Bus)
            .Include(t => t.Schedule)
                .ThenInclude(s => s.Route)
            .Where(t => t.UserId == userId && !t.IsDeleted &&
                   t.Schedule.DepartureTime > now &&
                   (t.Status == Domain.Enums.TicketStatus.Booked || t.Status == Domain.Enums.TicketStatus.Reserved))
            .OrderBy(t => t.Schedule.DepartureTime)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Ticket>> GetCompletedByUserAsync(Guid userId)
    {
        var now = DateTime.UtcNow;
        return await _context.Tickets
            .Include(t => t.Schedule)
                .ThenInclude(s => s.Bus)
            .Include(t => t.Schedule)
                .ThenInclude(s => s.Route)
            .Where(t => t.UserId == userId && !t.IsDeleted &&
                   (t.Status == Domain.Enums.TicketStatus.Completed || t.Schedule.DepartureTime < now))
            .OrderByDescending(t => t.Schedule.DepartureTime)
            .ToListAsync();
    }
    
    public async Task<Ticket> CreateAsync(Ticket ticket)
    {
        ticket.CreatedAt = DateTime.UtcNow;
        ticket.BookingReference = await GenerateBookingReferenceAsync();
        ticket.BookingDate = DateTime.UtcNow;
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();
        return ticket;
    }
    
    public async Task<Ticket> UpdateAsync(Ticket ticket)
    {
        ticket.UpdatedAt = DateTime.UtcNow;
        _context.Tickets.Update(ticket);
        await _context.SaveChangesAsync();
        return ticket;
    }
    
    public async Task<bool> DeleteAsync(Guid id)
    {
        var ticket = await GetByIdAsync(id);
        if (ticket == null) return false;
        
        ticket.IsDeleted = true;
        ticket.UpdatedAt = DateTime.UtcNow;
        _context.Tickets.Update(ticket);
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Tickets
            .AnyAsync(t => t.Id == id && !t.IsDeleted);
    }
    
    public async Task<bool> IsSeatBookedAsync(Guid scheduleId, int seatNumber)
    {
        return await _context.Tickets
            .AnyAsync(t => t.ScheduleId == scheduleId && t.SeatNumber == seatNumber &&
                   !t.IsDeleted && t.Status != Domain.Enums.TicketStatus.Cancelled);
    }
    
    public async Task<IEnumerable<int>> GetBookedSeatsAsync(Guid scheduleId)
    {
        return await _context.Tickets
            .Where(t => t.ScheduleId == scheduleId && !t.IsDeleted &&
                   t.Status != Domain.Enums.TicketStatus.Cancelled)
            .Select(t => t.SeatNumber)
            .ToListAsync();
    }
    
    public async Task<string> GenerateBookingReferenceAsync()
    {
        var random = new Random();
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var reference = new string(Enumerable.Range(0, 8).Select(_ => chars[random.Next(chars.Length)]).ToArray());
        
        // Ensure uniqueness
        while (await _context.Tickets.AnyAsync(t => t.BookingReference == reference))
        {
            reference = new string(Enumerable.Range(0, 8).Select(_ => chars[random.Next(chars.Length)]).ToArray());
        }
        
        return reference;
    }
}

/// <summary>
/// Payment repository implementation
/// </summary>
public class PaymentRepository : IPaymentRepository
{
    private readonly ApplicationDbContext _context;
    
    public PaymentRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Payment?> GetByIdAsync(Guid id)
    {
        return await _context.Payments
            .Include(p => p.Ticket)
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
    
    public async Task<Payment?> GetByTransactionIdAsync(string transactionId)
    {
        return await _context.Payments
            .Include(p => p.Ticket)
            .FirstOrDefaultAsync(p => p.TransactionId == transactionId);
    }
    
    public async Task<IEnumerable<Payment>> GetAllAsync()
    {
        return await _context.Payments
            .Include(p => p.Ticket)
            .Include(p => p.User)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Payment>> GetByUserAsync(Guid userId)
    {
        return await _context.Payments
            .Include(p => p.Ticket)
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Payment>> GetByTicketAsync(Guid ticketId)
    {
        return await _context.Payments
            .Where(p => p.TicketId == ticketId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }
    
    public async Task<Payment> CreateAsync(Payment payment)
    {
        payment.CreatedAt = DateTime.UtcNow;
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();
        return payment;
    }
    
    public async Task<Payment> UpdateAsync(Payment payment)
    {
        _context.Payments.Update(payment);
        await _context.SaveChangesAsync();
        return payment;
    }
    
    public async Task<IEnumerable<Payment>> GetPaymentsByDateRangeAsync(DateTime from, DateTime to)
    {
        return await _context.Payments
            .Include(p => p.Ticket)
            .Include(p => p.User)
            .Where(p => p.CreatedAt >= from && p.CreatedAt <= to)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }
}

/// <summary>
/// SeatLock repository implementation
/// </summary>
public class SeatLockRepository : ISeatLockRepository
{
    private readonly ApplicationDbContext _context;
    
    public SeatLockRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<SeatLock?> GetByIdAsync(Guid id)
    {
        return await _context.SeatLocks
            .FirstOrDefaultAsync(sl => sl.Id == id);
    }
    
    public async Task<SeatLock?> GetActiveLockAsync(Guid scheduleId, int seatNumber)
    {
        var now = DateTime.UtcNow;
        return await _context.SeatLocks
            .FirstOrDefaultAsync(sl => sl.ScheduleId == scheduleId &&
                                 sl.SeatNumber == seatNumber &&
                                 sl.ExpiresAt > now &&
                                 sl.Status == SeatLockStatus.Locked);
    }
    
    public async Task<SeatLock?> GetUserActiveLockAsync(Guid scheduleId, Guid userId)
    {
        var now = DateTime.UtcNow;
        return await _context.SeatLocks
            .FirstOrDefaultAsync(sl => sl.ScheduleId == scheduleId &&
                                 sl.UserId == userId &&
                                 sl.ExpiresAt > now &&
                                 sl.Status == SeatLockStatus.Locked);
    }
    
    public async Task<IEnumerable<SeatLock>> GetExpiredLocksAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.SeatLocks
            .Where(sl => sl.ExpiresAt < now && sl.Status == SeatLockStatus.Locked)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<SeatLock>> GetScheduleLocksAsync(Guid scheduleId)
    {
        var now = DateTime.UtcNow;
        return await _context.SeatLocks
            .Where(sl => sl.ScheduleId == scheduleId && sl.ExpiresAt > now)
            .ToListAsync();
    }
    
    public async Task<SeatLock> CreateAsync(SeatLock seatLock)
    {
        seatLock.CreatedAt = DateTime.UtcNow;
        _context.SeatLocks.Add(seatLock);
        await _context.SaveChangesAsync();
        return seatLock;
    }
    
    public async Task<SeatLock> UpdateAsync(SeatLock seatLock)
    {
        _context.SeatLocks.Update(seatLock);
        await _context.SaveChangesAsync();
        return seatLock;
    }
    
    public async Task<bool> DeleteAsync(Guid id)
    {
        var lockItem = await GetByIdAsync(id);
        if (lockItem == null) return false;
        
        _context.SeatLocks.Remove(lockItem);
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task ReleaseExpiredLocksAsync()
    {
        var expiredLocks = await GetExpiredLocksAsync();
        foreach (var lockItem in expiredLocks)
        {
            lockItem.Status = SeatLockStatus.Available;
            _context.SeatLocks.Update(lockItem);
        }
        await _context.SaveChangesAsync();
    }
    
    public async Task ReleaseUserLocksAsync(Guid userId, string sessionId)
    {
        var userLocks = await _context.SeatLocks
            .Where(sl => sl.UserId == userId && sl.SessionId == sessionId && sl.Status == SeatLockStatus.Locked)
            .ToListAsync();
        
        foreach (var lockItem in userLocks)
        {
            lockItem.Status = SeatLockStatus.Available;
            _context.SeatLocks.Update(lockItem);
        }
        await _context.SaveChangesAsync();
    }
}
