using OmniBus.Domain.Common;
using OmniBus.Domain.Enums;

namespace OmniBus.Domain.Entities;

/// <summary>
/// Represents a payment transaction
/// </summary>
public class Payment : BaseEntity
{
    /// <summary>
    /// The ticket this payment is for
    /// </summary>
    public Guid TicketId { get; set; }
    public virtual Ticket Ticket { get; set; } = null!;
    
    /// <summary>
    /// User who made the payment
    /// </summary>
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
    
    /// <summary>
    /// Payment amount
    /// </summary>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// Currency code
    /// </summary>
    public string Currency { get; set; } = "TND";
    
    /// <summary>
    /// Current status of the payment
    /// </summary>
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    
    /// <summary>
    /// Payment method used
    /// </summary>
    public string? PaymentMethod { get; set; }
    
    /// <summary>
    /// External transaction ID (from payment gateway)
    /// </summary>
    public string? TransactionId { get; set; }
    
    /// <summary>
    /// Payment gateway response
    /// </summary>
    public string? GatewayResponse { get; set; }
    
    /// <summary>
    /// When the payment was initiated
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// When the payment was completed
    /// </summary>
    public DateTime? CompletedAt { get; set; }
    
    /// <summary>
    /// Payment failure reason
    /// </summary>
    public string? FailureReason { get; set; }
    
    /// <summary>
    /// Refund amount if applicable
    /// </summary>
    public decimal? RefundAmount { get; set; }
    
    /// <summary>
    /// When the refund was processed
    /// </summary>
    public DateTime? RefundedAt { get; set; }
}
