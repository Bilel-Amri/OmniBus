using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmniBus.Application.DTOs;
using OmniBus.Application.Interfaces;

namespace OmniBus.API.Controllers;

/// <summary>
/// Controller for payment management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    
    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }
    
    /// <summary>
    /// Process a payment
    /// </summary>
    [HttpPost("process")]
    [Authorize]
    [ProducesResponseType(typeof(PaymentResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ProcessPayment([FromBody] CreatePaymentDto request)
    {
        var userId = GetUserIdFromClaims();
        if (userId == null)
            return Unauthorized();
        
        var result = await _paymentService.ProcessPaymentAsync(userId.Value, request);
        if (!result.Success)
            return BadRequest(new { message = result.ErrorMessage });
        
        return Ok(result);
    }
    
    /// <summary>
    /// Get payment by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(PaymentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPaymentById(Guid id)
    {
        var payment = await _paymentService.GetPaymentByIdAsync(id);
        if (payment == null)
            return NotFound();
        
        // Check access
        var userId = GetUserIdFromClaims();
        var userRole = GetUserRoleFromClaims();
        
        if (userRole != Domain.Enums.UserRole.Admin && payment.UserId != userId)
        {
            return Forbid();
        }
        
        return Ok(payment);
    }
    
    /// <summary>
    /// Get my payments
    /// </summary>
    [HttpGet("my-payments")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<PaymentResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyPayments()
    {
        var userId = GetUserIdFromClaims();
        if (userId == null)
            return Unauthorized();
        
        var payments = await _paymentService.GetPaymentsByUserAsync(userId.Value);
        return Ok(payments);
    }
    
    /// <summary>
    /// Get payments by ticket
    /// </summary>
    [HttpGet("ticket/{ticketId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<PaymentResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaymentsByTicket(Guid ticketId)
    {
        var payments = await _paymentService.GetPaymentsByTicketAsync(ticketId);
        return Ok(payments);
    }
    
    /// <summary>
    /// Refund a payment (Admin only)
    /// </summary>
    [HttpPost("{id:guid}/refund")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(PaymentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefundPayment(Guid id, [FromBody] RefundRequestDto request)
    {
        var payment = await _paymentService.RefundPaymentAsync(id, request);
        if (payment == null)
            return NotFound();
        
        return Ok(payment);
    }
    
    /// <summary>
    /// Get payment statistics (Admin only)
    /// </summary>
    [HttpGet("stats")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(PaymentStatsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaymentStats()
    {
        var stats = await _paymentService.GetPaymentStatsAsync();
        return Ok(stats);
    }
    
    /// <summary>
    /// Get payments by date range (Admin only)
    /// </summary>
    [HttpGet("range")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<PaymentResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaymentsByDateRange([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var payments = await _paymentService.GetPaymentsByDateRangeAsync(from, to);
        return Ok(payments);
    }
    
    /// <summary>
    /// Payment gateway callback (webhook)
    /// </summary>
    [HttpPost("callback")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> PaymentCallback([FromQuery] string transactionId, [FromQuery] string status, [FromBody] string gatewayResponse)
    {
        var success = await _paymentService.HandlePaymentCallbackAsync(transactionId, status, gatewayResponse);
        return Ok(new { success });
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
    
    private Domain.Enums.UserRole GetUserRoleFromClaims()
    {
        var roleClaim = User.FindFirst("role") ?? User.FindFirst(System.Security.Claims.ClaimTypes.Role);
        if (roleClaim != null && Enum.TryParse<Domain.Enums.UserRole>(roleClaim.Value, out var role))
        {
            return role;
        }
        return Domain.Enums.UserRole.Passenger;
    }
}
