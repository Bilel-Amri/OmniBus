using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmniBus.Application.DTOs;
using OmniBus.Application.Interfaces;

namespace OmniBus.API.Controllers;

/// <summary>
/// Controller for route management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RoutesController : ControllerBase
{
    private readonly IRouteService _routeService;
    
    public RoutesController(IRouteService routeService)
    {
        _routeService = routeService;
    }
    
    /// <summary>
    /// Get all routes
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RouteResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllRoutes()
    {
        var routes = await _routeService.GetAllRoutesAsync();
        return Ok(routes);
    }
    
    /// <summary>
    /// Get route by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RouteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRouteById(Guid id)
    {
        var route = await _routeService.GetRouteByIdAsync(id);
        if (route == null)
            return NotFound();
        
        return Ok(route);
    }
    
    /// <summary>
    /// Search routes
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<RouteResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchRoutes([FromQuery] string? origin, [FromQuery] string? destination)
    {
        if (string.IsNullOrEmpty(origin) && string.IsNullOrEmpty(destination))
        {
            return BadRequest(new { message = "At least origin or destination is required" });
        }
        
        var routes = await _routeService.SearchRoutesAsync(origin ?? "", destination ?? "");
        return Ok(routes);
    }
    
    /// <summary>
    /// Get active routes
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(IEnumerable<RouteResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveRoutes()
    {
        var routes = await _routeService.GetActiveRoutesAsync();
        return Ok(routes);
    }
    
    /// <summary>
    /// Get all unique cities from routes
    /// </summary>
    [HttpGet("cities")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCities()
    {
        var routes = await _routeService.GetAllRoutesAsync();
        var cities = routes
            .SelectMany(r => new[] { r.Origin, r.Destination })
            .Distinct()
            .OrderBy(c => c)
            .ToList();
        return Ok(cities);
    }
    
    /// <summary>
    /// Create a new route (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(RouteResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateRoute([FromBody] CreateRouteDto request)
    {
        var route = await _routeService.CreateRouteAsync(request);
        return CreatedAtAction(nameof(GetRouteById), new { id = route.Id }, route);
    }
    
    /// <summary>
    /// Update a route (Admin only)
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(RouteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateRoute(Guid id, [FromBody] UpdateRouteDto request)
    {
        var route = await _routeService.UpdateRouteAsync(id, request);
        if (route == null)
            return NotFound();
        
        return Ok(route);
    }
    
    /// <summary>
    /// Delete a route (Admin only)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRoute(Guid id)
    {
        var deleted = await _routeService.DeleteRouteAsync(id);
        if (!deleted)
            return NotFound();
        
        return NoContent();
    }
}
