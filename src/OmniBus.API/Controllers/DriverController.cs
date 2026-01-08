using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmniBus.Application.DTOs;
using OmniBus.Application.Interfaces;
using OmniBus.Domain.Enums;
using OmniBus.API.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace OmniBus.API.Controllers;

/// <summary>
/// Controller for driver-specific endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Driver")]
public class DriverController : ControllerBase
{
    private readonly IDriverService _driverService;
    private readonly IScheduleService _scheduleService;
    private readonly ITicketService _ticketService;
    private readonly IHubContext<TrackingHub> _trackingHub;
    
    public DriverController(IDriverService driverService, IScheduleService scheduleService, ITicketService ticketService, IHubContext<TrackingHub> trackingHub)
    {
        _driverService = driverService;
        _scheduleService = scheduleService;
        _ticketService = ticketService;
        _trackingHub = trackingHub;
    }
    
    /// <summary>
    /// Get current driver's assigned trips
    /// </summary>
    [HttpGet("trips")]
    [ProducesResponseType(typeof(IEnumerable<ScheduleResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyTrips()
    {
        var userId = GetUserIdFromClaims();
        if (userId == null)
            return Unauthorized();
        
        var schedules = await _scheduleService.GetSchedulesByDriverAsync(userId.Value);
        return Ok(schedules);
    }
    
    /// <summary>
    /// Get driver's today's trips
    /// </summary>
    [HttpGet("today")]
    [ProducesResponseType(typeof(IEnumerable<ScheduleResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTodaysTrips()
    {
        var userId = GetUserIdFromClaims();
        if (userId == null)
            return Unauthorized();
        
        var today = DateTime.Today;
        var schedules = await _scheduleService.GetSchedulesByDriverAsync(userId.Value);
        var todaySchedules = schedules.Where(s => s.DepartureTime.Date == today);
        
        return Ok(todaySchedules);
    }
    
    /// <summary>
    /// Update current location
    /// </summary>
    [HttpPut("location")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateLocation([FromBody] LocationUpdateDto location)
    {
        var userId = GetUserIdFromClaims();
        if (userId == null)
            return Unauthorized();
        if (!location.Timestamp.HasValue)
            location.Timestamp = DateTime.UtcNow;
        
        var success = await _driverService.UpdateDriverLocationAsync(userId.Value, location);
        if (!success)
            return BadRequest("Failed to update location");

        var schedules = await _scheduleService.GetSchedulesByDriverAsync(userId.Value);
        foreach (var schedule in schedules)
        {
            if (schedule.Route != null)
            {
                await _trackingHub.Clients.Group($"route_{schedule.Route.Id}")
                    .SendAsync("BusLocationUpdated", schedule.Id, location);
                await _trackingHub.Clients.Group($"schedule_{schedule.Id}")
                    .SendAsync("BusLocationUpdated", schedule.Id, location);
                await _trackingHub.Clients.Group("admin")
                    .SendAsync("BusLocationUpdated", schedule.Id, location);
            }
        }
        
        return Ok(new { message = "Location updated successfully" });
    }
    
    /// <summary>
    /// Get tickets for a schedule (to validate boarding)
    /// </summary>
    [HttpGet("schedule/{scheduleId:guid}/tickets")]
    [ProducesResponseType(typeof(IEnumerable<TicketResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetScheduleTickets(Guid scheduleId)
    {
        var tickets = await _ticketService.GetBookedSeatsAsync(scheduleId);
        return Ok(tickets);
    }
    
    /// <summary>
    /// Confirm passenger boarding
    /// </summary>
    [HttpPost("confirm-boarding")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmBoarding([FromBody] ConfirmBoardingDto request)
    {
        var userId = GetUserIdFromClaims();
        if (userId == null)
            return Unauthorized();
        
        var success = await _ticketService.ConfirmBoardingAsync(request.TicketId, userId.Value, request.QrCode);
        if (!success)
            return BadRequest("Failed to confirm boarding. Ticket may be invalid or already used.");
        
        return Ok(new { message = "Boarding confirmed successfully" });
    }
    
    /// <summary>
    /// Start trip (set status to InProgress)
    /// </summary>
    [HttpPut("schedule/{scheduleId:guid}/start")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> StartTrip(Guid scheduleId)
    {
        var userId = GetUserIdFromClaims();
        if (userId == null)
            return Unauthorized();
        
        var success = await _driverService.StartTripAsync(scheduleId, userId.Value);
        if (!success)
            return BadRequest("Failed to start trip");
        
        return Ok(new { message = "Trip started successfully" });
    }
    
    /// <summary>
    /// Complete trip (set status to Completed)
    /// </summary>
    [HttpPut("schedule/{scheduleId:guid}/complete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CompleteTrip(Guid scheduleId)
    {
        var userId = GetUserIdFromClaims();
        if (userId == null)
            return Unauthorized();
        
        var success = await _driverService.CompleteTripAsync(scheduleId, userId.Value);
        if (!success)
            return BadRequest("Failed to complete trip");
        
        return Ok(new { message = "Trip completed successfully" });
    }
    
    /// <summary>
    /// Report delay
    /// </summary>
    [HttpPut("schedule/{scheduleId:guid}/delay")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ReportDelay(Guid scheduleId, [FromBody] DelayReportDto request)
    {
        var userId = GetUserIdFromClaims();
        if (userId == null)
            return Unauthorized();
        
        var success = await _driverService.ReportDelayAsync(scheduleId, userId.Value, request.DelayMinutes, request.Reason);
        if (!success)
            return BadRequest("Failed to report delay");
        
        return Ok(new { message = "Delay reported successfully" });
    }
    
    private Guid? GetUserIdFromClaims()
    {
        var userIdClaim = User.FindFirst("nameid") ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }
        return null;
    }
}

/// <summary>
/// Delay report DTO
/// </summary>
public class DelayReportDto
{
    public int DelayMinutes { get; set; }
    public string? Reason { get; set; }
}
