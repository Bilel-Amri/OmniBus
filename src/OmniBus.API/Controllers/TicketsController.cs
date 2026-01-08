using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmniBus.Application.DTOs;
using OmniBus.Application.Interfaces;
using OmniBus.Domain.Enums;

namespace OmniBus.API.Controllers;

/// <summary>
/// Controller for ticket management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;
    private readonly IConfiguration _configuration;
    
    public TicketsController(ITicketService ticketService, IConfiguration configuration)
    {
        _ticketService = ticketService;
        _configuration = configuration;
    }
    
    /// <summary>
    /// Get all tickets (Admin only)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<TicketResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllTickets()
    {
        var tickets = await _ticketService.GetAllTicketsAsync();
        return Ok(tickets);
    }
    
    /// <summary>
    /// Get ticket by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(TicketResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTicketById(Guid id)
    {
        var ticket = await _ticketService.GetTicketByIdAsync(id);
        if (ticket == null)
            return NotFound();
        
        // Check if user has access
        var userId = GetUserIdFromClaims();
        if (userId == null)
            return Unauthorized();
        
        var userRole = GetUserRoleFromClaims();
        var ticketUserId = await GetTicketUserIdAsync(id);
        
        if (userRole != UserRole.Admin && ticketUserId != userId.Value)
        {
            return Forbid();
        }
        
        return Ok(ticket);
    }
    
    /// <summary>
    /// Get ticket by booking reference
    /// </summary>
    [HttpGet("reference/{reference}")]
    [ProducesResponseType(typeof(TicketResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTicketByReference(string reference)
    {
        var ticket = await _ticketService.GetTicketByReferenceAsync(reference);
        if (ticket == null)
            return NotFound();
        
        return Ok(ticket);
    }
    
    /// <summary>
    /// Get my tickets (authenticated user)
    /// </summary>
    [HttpGet("my-tickets")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<MyTicketDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyTickets()
    {
        var userId = GetUserIdFromClaims();
        if (userId == null)
            return Unauthorized();
        
        var tickets = await _ticketService.GetTicketsByUserAsync(userId.Value);
        return Ok(tickets);
    }
    
    /// <summary>
    /// Get upcoming tickets
    /// </summary>
    [HttpGet("upcoming")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<MyTicketDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUpcomingTickets()
    {
        var userId = GetUserIdFromClaims();
        if (userId == null)
            return Unauthorized();
        
        var tickets = await _ticketService.GetUpcomingTicketsByUserAsync(userId.Value);
        return Ok(tickets);
    }
    
    /// <summary>
    /// Get completed tickets
    /// </summary>
    [HttpGet("history")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<MyTicketDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCompletedTickets()
    {
        var userId = GetUserIdFromClaims();
        if (userId == null)
            return Unauthorized();
        
        var tickets = await _ticketService.GetCompletedTicketsByUserAsync(userId.Value);
        return Ok(tickets);
    }
    
    /// <summary>
    /// Lock a seat for booking
    /// </summary>
    [HttpPost("lock-seat")]
    [Authorize]
    [ProducesResponseType(typeof(SeatLockResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LockSeat([FromBody] SeatLockRequestDto request)
    {
        var userId = GetUserIdFromClaims();
        if (userId == null)
            return Unauthorized();
        
        var sessionId = Request.Headers["X-Session-Id"].ToString() ?? Guid.NewGuid().ToString();
        
        var lockResult = await _ticketService.LockSeatAsync(userId.Value, request, sessionId);
        if (lockResult == null)
            return BadRequest(new { message = "Seat is not available for locking" });
        
        return Ok(lockResult);
    }
    
    /// <summary>
    /// Release seat lock
    /// </summary>
    [HttpDelete("lock/{lockId:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ReleaseSeatLock(Guid lockId)
    {
        var released = await _ticketService.ReleaseSeatLockAsync(lockId);
        return Ok(new { success = released });
    }
    
    /// <summary>
    /// Book a ticket
    /// </summary>
    [HttpPost("book")]
    [Authorize]
    [ProducesResponseType(typeof(TicketResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BookTicket([FromBody] CreateTicketDto request)
    {
        var userId = GetUserIdFromClaims();
        if (userId == null)
            return Unauthorized();
        
        var ticket = await _ticketService.BookTicketAsync(userId.Value, request);
        return CreatedAtAction(nameof(GetTicketById), new { id = ticket.Id }, ticket);
    }
    
    /// <summary>
    /// Cancel a ticket
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    [Authorize]
    [ProducesResponseType(typeof(TicketResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelTicket(Guid id, [FromBody] CancelTicketDto request)
    {
        var userId = GetUserIdFromClaims();
        if (userId == null)
            return Unauthorized();
        
        var ticket = await _ticketService.CancelTicketAsync(id, request);
        if (ticket == null)
            return NotFound();
        
        return Ok(ticket);
    }
    
    /// <summary>
    /// Get ticket statistics (Admin)
    /// </summary>
    [HttpGet("stats")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(TicketStatsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTicketStats()
    {
        var stats = await _ticketService.GetTicketStatsAsync();
        return Ok(stats);
    }
    
    /// <summary>
    /// Get seat availability for a schedule
    /// </summary>
    [HttpGet("{scheduleId:guid}/availability")]
    [ProducesResponseType(typeof(ScheduleSeatLayoutDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSeatAvailability(Guid scheduleId)
    {
        var userId = GetUserIdFromClaims();
        var layout = await _ticketService.GetSeatLayoutWithAvailabilityAsync(scheduleId, userId);
        if (layout == null)
            return NotFound();
        
        return Ok(layout);
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
    
    private UserRole GetUserRoleFromClaims()
    {
        var roleClaim = User.FindFirst("role") ?? User.FindFirst(System.Security.Claims.ClaimTypes.Role);
        if (roleClaim != null && Enum.TryParse<UserRole>(roleClaim.Value, out var role))
        {
            return role;
        }
        return UserRole.Passenger;
    }
    
    private async Task<Guid?> GetTicketUserIdAsync(Guid ticketId)
    {
        var ticket = await _ticketService.GetTicketByIdAsync(ticketId);
        return ticket?.UserId;
    }
}
