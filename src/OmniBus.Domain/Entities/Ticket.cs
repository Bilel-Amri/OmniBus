using OmniBus.Domain.Common;
using OmniBus.Domain.Enums;

namespace OmniBus.Domain.Entities;

/// <summary>
/// Represents a booked ticket
/// </summary>
public class Ticket : AuditableEntity
{
    /// <summary>
    /// The passenger who booked this ticket
    /// </summary>
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
    
    /// <summary>
    /// The schedule this ticket is for
    /// </summary>
    public Guid ScheduleId { get; set; }
    public virtual Schedule Schedule { get; set; } = null!;
    
    /// <summary>
    /// Seat number assigned
    /// </summary>
    public int SeatNumber { get; set; }
    
    /// <summary>
    /// Current status of the ticket
    /// </summary>
    public TicketStatus Status { get; set; } = TicketStatus.Booked;
    
    /// <summary>
    /// Price paid for this ticket
    /// </summary>
    public decimal Price { get; set; }
    
    /// <summary>
    /// Booking reference number (unique)
    /// </summary>
    public string BookingReference { get; set; } = string.Empty;
    
    /// <summary>
    /// When the ticket was booked
    /// </summary>
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Payment ID if paid
    /// </summary>
    public Guid? PaymentId { get; set; }
    public virtual Payment? Payment { get; set; }
    
    /// <summary>
    /// QR code content for ticket validation
    /// </summary>
    public string? QrCode { get; set; }
    
    /// <summary>
    /// Passenger's name (for display on ticket)
    /// </summary>
    public string PassengerName { get; set; } = string.Empty;
    
    /// <summary>
    /// Passenger's phone number
    /// </summary>
    public string? PassengerPhone { get; set; }
    
    /// <summary>
    /// When the passenger boarded the bus
    /// </summary>
    public DateTime? BoardingTime { get; set; }
    
    /// <summary>
    /// Who confirmed the boarding
    /// </summary>
    public string? BoardedBy { get; set; }
    
    /// <summary>
    /// Cancellation reason if cancelled
    /// </summary>
    public string? CancellationReason { get; set; }
    
    /// <summary>
    /// When the ticket was cancelled
    /// </summary>
    public DateTime? CancelledAt { get; set; }
}
