using OmniBus.Domain.Common;
using OmniBus.Domain.Enums;

namespace OmniBus.Domain.Entities;

/// <summary>
/// Represents a temporary seat lock during booking process
/// Prevents double-booking when multiple users try to book the same seat
/// </summary>
public class SeatLock : BaseEntity
{
    /// <summary>
    /// The schedule this lock is for
    /// </summary>
    public Guid ScheduleId { get; set; }
    public virtual Schedule Schedule { get; set; } = null!;
    
    /// <summary>
    /// The user who locked the seat
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// Seat number that is locked
    /// </summary>
    public int SeatNumber { get; set; }
    
    /// <summary>
    /// When the lock was created
    /// </summary>
    public DateTime LockedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// When the lock expires
    /// </summary>
    public DateTime ExpiresAt { get; set; }
    
    /// <summary>
    /// Current status of the lock
    /// </summary>
    public SeatLockStatus Status { get; set; } = SeatLockStatus.Locked;
    
    /// <summary>
    /// Session ID for the user
    /// </summary>
    public string SessionId { get; set; } = string.Empty;
    
    /// <summary>
    /// Check if the lock has expired
    /// </summary>
    public bool IsExpired => DateTime.UtcNow > ExpiresAt;
    
    /// <summary>
    /// Check if the lock is still valid
    /// </summary>
    public bool IsValid => !IsExpired && Status == SeatLockStatus.Locked;
}
