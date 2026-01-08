using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmniBus.Infrastructure.Persistence;
using OmniBus.Infrastructure.Services;
using OmniBus.Domain.Enums;

namespace OmniBus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ISmsService _smsService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(ApplicationDbContext context, ISmsService smsService, ILogger<AdminController> logger)
    {
        _context = context;
        _smsService = smsService;
        _logger = logger;
    }

    /// <summary>
    /// Bulk cancel trip - handles bus breakdown scenarios
    /// Updates schedule status, notifies passengers via SMS, queues refunds
    /// </summary>
    [HttpPost("trips/{scheduleId}/cancel")]
    public async Task<IActionResult> CancelTrip(Guid scheduleId, [FromBody] CancelTripRequest request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            // Get schedule
            var schedule = await _context.Schedules
                .Include(s => s.Route)
                .FirstOrDefaultAsync(s => s.Id == scheduleId);

            if (schedule == null)
                return NotFound(new { message = "Schedule not found" });

            // Update schedule status
            schedule.Status = ScheduleStatus.Cancelled;
            schedule.UpdatedAt = DateTime.UtcNow;

            // Get all tickets for this schedule
            var tickets = await _context.Tickets
                .Include(t => t.User)
                .Include(t => t.Payment)
                .Where(t => t.ScheduleId == scheduleId && 
                           (t.Status == TicketStatus.Booked || t.Status == TicketStatus.Reserved))
                .ToListAsync();

            var notificationTasks = new List<Task<bool>>();
            var refundCount = 0;

            foreach (var ticket in tickets)
            {
                // Update ticket status
                ticket.Status = TicketStatus.Cancelled;
                ticket.UpdatedAt = DateTime.UtcNow;

                // Queue payment for refund
                if (ticket.Payment != null && ticket.Payment.Status == PaymentStatus.Completed)
                {
                    ticket.Payment.Status = PaymentStatus.Refunded;
                    ticket.Payment.UpdatedAt = DateTime.UtcNow;
                    refundCount++;
                }

                // Send SMS notification
                if (!string.IsNullOrEmpty(ticket.User.PhoneNumber))
                {
                    var smsTask = _smsService.SendCancellationNoticeAsync(
                        ticket.User.PhoneNumber,
                        ticket.BookingReference,
                        request.Reason ?? "Annulation du voyage"
                    );
                    notificationTasks.Add(smsTask);
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            // Send SMS notifications (don't await all - let them run async)
            _ = Task.WhenAll(notificationTasks).ContinueWith(task =>
            {
                var successCount = task.Result.Count(r => r);
                _logger.LogInformation(
                    "Trip {ScheduleId} cancelled. SMS sent: {Success}/{Total}",
                    scheduleId, successCount, notificationTasks.Count
                );
            });

            return Ok(new
            {
                message = "Trip cancelled successfully",
                scheduleId,
                ticketsCancelled = tickets.Count,
                refundsQueued = refundCount,
                smsNotifications = notificationTasks.Count
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Failed to cancel trip {ScheduleId}", scheduleId);
            return StatusCode(500, new { message = "Failed to cancel trip", error = ex.Message });
        }
    }

    /// <summary>
    /// Get trip cancellation summary
    /// </summary>
    [HttpGet("trips/{scheduleId}/cancellation-impact")]
    public async Task<IActionResult> GetCancellationImpact(Guid scheduleId)
    {
        var tickets = await _context.Tickets
            .Include(t => t.Payment)
            .Where(t => t.ScheduleId == scheduleId && 
                       (t.Status == TicketStatus.Booked || t.Status == TicketStatus.Reserved))
            .ToListAsync();

        var totalRefundAmount = tickets
            .Where(t => t.Payment != null)
            .Sum(t => t.Payment!.Amount);

        return Ok(new
        {
            scheduleId,
            affectedPassengers = tickets.Count,
            totalRefundAmount,
            bookedTickets = tickets.Count(t => t.Status == TicketStatus.Booked),
            reservedTickets = tickets.Count(t => t.Status == TicketStatus.Reserved)
        });
    }

    /// <summary>
    /// Bulk refund processing
    /// </summary>
    [HttpPost("payments/process-refunds")]
    public async Task<IActionResult> ProcessPendingRefunds()
    {
        var pendingRefunds = await _context.Payments
            .Where(p => p.Status == PaymentStatus.Refunded && p.RefundedAt == null)
            .Take(50) // Process in batches
            .ToListAsync();

        var processed = 0;
        var failed = 0;

        foreach (var payment in pendingRefunds)
        {
            try
            {
                // Mark as refund processed
                payment.RefundedAt = DateTime.UtcNow;
                payment.UpdatedAt = DateTime.UtcNow;
                processed++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Refund failed for payment {PaymentId}", payment.Id);
                failed++;
            }
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            totalPending = pendingRefunds.Count,
            processed,
            failed
        });
    }

    /// <summary>
    /// Generate driver manifest for offline validation
    /// </summary>
    [HttpGet("schedules/{scheduleId}/manifest")]
    public async Task<IActionResult> GetDriverManifest(Guid scheduleId)
    {
        var schedule = await _context.Schedules
            .Include(s => s.Route)
            .Include(s => s.Bus)
            .FirstOrDefaultAsync(s => s.Id == scheduleId);

        if (schedule == null)
            return NotFound();

        var tickets = await _context.Tickets
            .Include(t => t.User)
            .Where(t => t.ScheduleId == scheduleId && t.Status == TicketStatus.Booked)
            .OrderBy(t => t.SeatNumber)
            .Select(t => new
            {
                ticketId = t.Id,
                bookingReference = t.BookingReference,
                passengerName = $"{t.User.FirstName} {t.User.LastName}",
                phoneNumber = t.User.PhoneNumber,
                seatNumber = t.SeatNumber,
                qrCodeData = t.QrCode
            })
            .ToListAsync();

        return Ok(new
        {
            scheduleId = schedule.Id,
            route = new
            {
                name = schedule.Route.Name,
                origin = schedule.Route.Origin,
                destination = schedule.Route.Destination
            },
            bus = new
            {
                registrationNumber = schedule.Bus?.RegistrationNumber,
                capacity = schedule.Bus?.Capacity
            },
            departureTime = schedule.DepartureTime,
            totalPassengers = tickets.Count,
            passengers = tickets,
            generatedAt = DateTime.UtcNow
        });
    }
}

public class CancelTripRequest
{
    public string? Reason { get; set; }
}
