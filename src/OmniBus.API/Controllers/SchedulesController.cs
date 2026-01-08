using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmniBus.Application.DTOs;
using OmniBus.Application.Interfaces;
using OmniBus.API.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace OmniBus.API.Controllers;

/// <summary>
/// Controller for schedule management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SchedulesController : ControllerBase
{
    private readonly IScheduleService _scheduleService;
    private readonly IHubContext<TrackingHub> _trackingHub;
    
    public SchedulesController(IScheduleService scheduleService, IHubContext<TrackingHub> trackingHub)
    {
        _scheduleService = scheduleService;
        _trackingHub = trackingHub;
    }
    
    /// <summary>
    /// Get all schedules
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ScheduleResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllSchedules()
    {
        var schedules = await _scheduleService.GetAllSchedulesAsync();
        return Ok(schedules);
    }
    
    /// <summary>
    /// Get schedule by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ScheduleResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetScheduleById(Guid id)
    {
        var schedule = await _scheduleService.GetScheduleByIdAsync(id);
        if (schedule == null)
            return NotFound();
        
        return Ok(schedule);
    }
    
    /// <summary>
    /// Search schedules
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<ScheduleResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchSchedules([FromQuery] SearchScheduleDto request)
    {
        var schedules = await _scheduleService.SearchSchedulesAsync(request);
        return Ok(schedules);
    }
    
    /// <summary>
    /// Get schedules by date
    /// </summary>
    [HttpGet("date/{date:datetime}")]
    [ProducesResponseType(typeof(IEnumerable<ScheduleResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSchedulesByDate(DateTime date)
    {
        var schedules = await _scheduleService.GetSchedulesByDateAsync(date);
        return Ok(schedules);
    }
    
    /// <summary>
    /// Get seat layout for a schedule
    /// </summary>
    [HttpGet("{id:guid}/seats")]
    [ProducesResponseType(typeof(ScheduleSeatLayoutDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSeatLayout(Guid id)
    {
        var layout = await _scheduleService.GetSeatLayoutAsync(id);
        if (layout == null)
            return NotFound();
        
        return Ok(layout);
    }
    
    /// <summary>
    /// Get active schedules with location
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(IEnumerable<ScheduleResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveSchedulesWithLocation()
    {
        var schedules = await _scheduleService.GetActiveSchedulesWithLocationAsync();
        return Ok(schedules);
    }
    
    /// <summary>
    /// Get schedules by route
    /// </summary>
    [HttpGet("route/{routeId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<ScheduleResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSchedulesByRoute(Guid routeId)
    {
        var schedules = await _scheduleService.GetSchedulesByRouteAsync(routeId);
        return Ok(schedules);
    }
    
    /// <summary>
    /// Create a new schedule (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ScheduleResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduleDto request)
    {
        var schedule = await _scheduleService.CreateScheduleAsync(request);
        return CreatedAtAction(nameof(GetScheduleById), new { id = schedule.Id }, schedule);
    }
    
    /// <summary>
    /// Update a schedule (Admin only)
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ScheduleResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateSchedule(Guid id, [FromBody] UpdateScheduleDto request)
    {
        var schedule = await _scheduleService.UpdateScheduleAsync(id, request);
        if (schedule == null)
            return NotFound();
        
        return Ok(schedule);
    }
    
    /// <summary>
    /// Delete a schedule (Admin only)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSchedule(Guid id)
    {
        var deleted = await _scheduleService.DeleteScheduleAsync(id);
        if (!deleted)
            return NotFound();
        
        return NoContent();
    }
    
    /// <summary>
    /// Update schedule location (Driver)
    /// </summary>
    [HttpPut("{id:guid}/location")]
    [Authorize(Roles = "Driver,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateScheduleLocation(Guid id, [FromBody] LocationUpdateDto location)
    {
        if (!location.Timestamp.HasValue)
            location.Timestamp = DateTime.UtcNow;
        var updated = await _scheduleService.UpdateScheduleLocationAsync(id, location);
        if (!updated)
            return BadRequest("Failed to update location");
        var schedule = await _scheduleService.GetScheduleByIdAsync(id);
        if (schedule?.Route != null)
        {
            await _trackingHub.Clients.Group($"route_{schedule.Route.Id}")
                .SendAsync("BusLocationUpdated", schedule.Id, location);
            await _trackingHub.Clients.Group($"schedule_{schedule.Id}")
                .SendAsync("BusLocationUpdated", schedule.Id, location);
            await _trackingHub.Clients.Group("admin")
                .SendAsync("BusLocationUpdated", schedule.Id, location);
        }
        
        return Ok(new { message = "Location updated successfully" });
    }
}
