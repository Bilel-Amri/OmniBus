namespace OmniBus.Application.DTOs;

public class AnalyticsDto
{
    public RevenueStatsDto Revenue { get; set; } = new();
    public List<DailyBookingDto> DailyBookings { get; set; } = new();
    public List<RoutePopularityDto> RoutePopularity { get; set; } = new();
    public List<OccupancyRateDto> OccupancyRates { get; set; } = new();
    public SystemStatsDto SystemStats { get; set; } = new();
}

public class RevenueStatsDto
{
    public decimal TotalRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public decimal DailyRevenue { get; set; }
    public decimal GrowthRate { get; set; }
    public List<MonthlyRevenueDto> MonthlyData { get; set; } = new();
}

public class MonthlyRevenueDto
{
    public string Month { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public int Bookings { get; set; }
}

public class DailyBookingDto
{
    public string Date { get; set; } = string.Empty;
    public int Bookings { get; set; }
    public int Cancellations { get; set; }
}

public class RoutePopularityDto
{
    public Guid RouteId { get; set; }
    public string RouteName { get; set; } = string.Empty;
    public int TotalBookings { get; set; }
    public decimal Revenue { get; set; }
    public double AverageOccupancy { get; set; }
}

public class OccupancyRateDto
{
    public Guid ScheduleId { get; set; }
    public string RouteName { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public int TotalSeats { get; set; }
    public int BookedSeats { get; set; }
    public double OccupancyPercentage { get; set; }
}

public class SystemStatsDto
{
    public int TotalUsers { get; set; }
    public int TotalDrivers { get; set; }
    public int TotalBuses { get; set; }
    public int TotalRoutes { get; set; }
    public int ActiveSchedules { get; set; }
    public int PendingPayments { get; set; }
}
