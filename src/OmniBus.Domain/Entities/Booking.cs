using OmniBus.Domain.Common;
using OmniBus.Domain.Enums;

namespace OmniBus.Domain.Entities;

/// <summary>
/// Represents a booking (can have multiple tickets/seats)
/// </summary>
public class Booking : AuditableEntity
{
    /// <summary>
    /// User who made the booking
    /// </summary>
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
    
    /// <summary>
    /// Schedule being booked
    /// </summary>
    public Guid ScheduleId { get; set; }
    public virtual Schedule Schedule { get; set; } = null!;
    
    /// <summary>
    /// Unique booking reference
    /// </summary>
    public string BookingReference { get; set; } = string.Empty;
    
    /// <summary>
    /// Current status
    /// </summary>
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    
    /// <summary>
    /// Total amount for all tickets in this booking
    /// </summary>
    public decimal TotalAmount { get; set; }
    
    /// <summary>
    /// Number of seats/tickets in this booking
    /// </summary>
    public int NumberOfSeats { get; set; }
    
    /// <summary>
    /// Booking date
    /// </summary>
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Contact email
    /// </summary>
    public string ContactEmail { get; set; } = string.Empty;
    
    /// <summary>
    /// Contact phone
    /// </summary>
    public string? ContactPhone { get; set; }
    
    /// <summary>
    /// Tickets in this booking
    /// </summary>
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    
    /// <summary>
    /// Payment for this booking
    /// </summary>
    public Guid? PaymentId { get; set; }
    public virtual Payment? Payment { get; set; }
}
