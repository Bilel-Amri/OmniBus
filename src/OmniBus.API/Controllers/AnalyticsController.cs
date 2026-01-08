using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmniBus.Application.Interfaces;

namespace OmniBus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboardAnalytics()
    {
        var analytics = await _analyticsService.GetDashboardAnalyticsAsync();
        return Ok(analytics);
    }

    [HttpGet("revenue")]
    public async Task<IActionResult> GetRevenueStats([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var revenue = await _analyticsService.GetRevenueStatsAsync(startDate, endDate);
        return Ok(revenue);
    }

    [HttpGet("daily-bookings")]
    public async Task<IActionResult> GetDailyBookings([FromQuery] int days = 30)
    {
        var bookings = await _analyticsService.GetDailyBookingsAsync(days);
        return Ok(bookings);
    }

    [HttpGet("route-popularity")]
    public async Task<IActionResult> GetRoutePopularity([FromQuery] int topN = 10)
    {
        var popularity = await _analyticsService.GetRoutePopularityAsync(topN);
        return Ok(popularity);
    }

    [HttpGet("occupancy-rates")]
    public async Task<IActionResult> GetOccupancyRates([FromQuery] DateTime? date)
    {
        var occupancy = await _analyticsService.GetOccupancyRatesAsync(date);
        return Ok(occupancy);
    }

    [HttpGet("system-stats")]
    public async Task<IActionResult> GetSystemStats()
    {
        var stats = await _analyticsService.GetSystemStatsAsync();
        return Ok(stats);
    }
}
