using OmniBus.Domain.Common;
using OmniBus.Domain.Enums;

namespace OmniBus.Domain.Entities;

/// <summary>
/// Represents a bus driver
/// </summary>
public class Driver : AuditableEntity
{
    /// <summary>
    /// User account for this driver
    /// </summary>
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
    
    /// <summary>
    /// Driver's license number
    /// </summary>
    public string LicenseNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// License expiry date
    /// </summary>
    public DateTime LicenseExpiryDate { get; set; }
    
    /// <summary>
    /// Driver's experience in years
    /// </summary>
    public int YearsOfExperience { get; set; }
    
    /// <summary>
    /// Current status
    /// </summary>
    public DriverStatus Status { get; set; } = DriverStatus.Active;
    
    /// <summary>
    /// Date hired
    /// </summary>
    public DateTime HireDate { get; set; }
    
    /// <summary>
    /// Emergency contact name
    /// </summary>
    public string? EmergencyContactName { get; set; }
    
    /// <summary>
    /// Emergency contact phone
    /// </summary>
    public string? EmergencyContactPhone { get; set; }
    
    /// <summary>
    /// Current location (latitude)
    /// </summary>
    public double? CurrentLatitude { get; set; }
    
    /// <summary>
    /// Current location (longitude)
    /// </summary>
    public double? CurrentLongitude { get; set; }
    
    /// <summary>
    /// Last location update
    /// </summary>
    public DateTime? LastLocationUpdate { get; set; }
    
    /// <summary>
    /// Whether driver is currently on duty
    /// </summary>
    public bool IsOnDuty { get; set; } = false;
    
    /// <summary>
    /// Current bus assignment
    /// </summary>
    public Guid? CurrentBusId { get; set; }
    public virtual Bus? CurrentBus { get; set; }
    
    /// <summary>
    /// Schedules assigned to this driver
    /// </summary>
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
