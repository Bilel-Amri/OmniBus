using OmniBus.Application.DTOs;

namespace OmniBus.Application.Interfaces;

public interface IAnalyticsService
{
    Task<AnalyticsDto> GetDashboardAnalyticsAsync();
    Task<RevenueStatsDto> GetRevenueStatsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<List<DailyBookingDto>> GetDailyBookingsAsync(int days = 30);
    Task<List<RoutePopularityDto>> GetRoutePopularityAsync(int topN = 10);
    Task<List<OccupancyRateDto>> GetOccupancyRatesAsync(DateTime? date = null);
    Task<SystemStatsDto> GetSystemStatsAsync();
}
