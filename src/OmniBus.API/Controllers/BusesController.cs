using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmniBus.Application.DTOs;
using OmniBus.Application.Interfaces;
using OmniBus.Domain.Enums;

namespace OmniBus.API.Controllers;

/// <summary>
/// Controller for bus management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BusesController : ControllerBase
{
    private readonly IBusService _busService;
    
    public BusesController(IBusService busService)
    {
        _busService = busService;
    }
    
    /// <summary>
    /// Get all buses
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BusResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllBuses()
    {
        var buses = await _busService.GetAllBusesAsync();
        return Ok(buses);
    }
    
    /// <summary>
    /// Get bus by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BusResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBusById(Guid id)
    {
        var bus = await _busService.GetBusByIdAsync(id);
        if (bus == null)
            return NotFound();
        
        return Ok(bus);
    }
    
    /// <summary>
    /// Create a new bus (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(BusResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateBus([FromBody] CreateBusDto request)
    {
        var bus = await _busService.CreateBusAsync(request);
        return CreatedAtAction(nameof(GetBusById), new { id = bus.Id }, bus);
    }
    
    /// <summary>
    /// Update a bus (Admin only)
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(BusResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateBus(Guid id, [FromBody] UpdateBusDto request)
    {
        var bus = await _busService.UpdateBusAsync(id, request);
        if (bus == null)
            return NotFound();
        
        return Ok(bus);
    }
    
    /// <summary>
    /// Delete a bus (Admin only)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBus(Guid id)
    {
        var deleted = await _busService.DeleteBusAsync(id);
        if (!deleted)
            return NotFound();
        
        return NoContent();
    }
    
    /// <summary>
    /// Get buses by status
    /// </summary>
    [HttpGet("status/{status}")]
    [ProducesResponseType(typeof(IEnumerable<BusResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBusesByStatus([FromRoute] BusStatus status)
    {
        var buses = await _busService.GetBusesByStatusAsync(status);
        return Ok(buses);
    }
    
    /// <summary>
    /// Get buses by type
    /// </summary>
    [HttpGet("type/{type}")]
    [ProducesResponseType(typeof(IEnumerable<BusResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBusesByType([FromRoute] BusType type)
    {
        var buses = await _busService.GetBusesByTypeAsync(type);
        return Ok(buses);
    }
    
    /// <summary>
    /// Get active buses with current locations
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(IEnumerable<BusResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveBusesWithLocation()
    {
        var buses = await _busService.GetActiveBusesWithLocationAsync();
        return Ok(buses);
    }
}
