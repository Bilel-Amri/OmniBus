using OmniBus.Domain.Common;
using OmniBus.Domain.Enums;

namespace OmniBus.Domain.Entities;

/// <summary>
/// Represents a bus in the fleet
/// </summary>
public class Bus : AuditableEntity
{
    /// <summary>
    /// License plate number (unique)
    /// </summary>
    public string PlateNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// Bus registration number
    /// </summary>
    public string RegistrationNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// Bus brand/make
    /// </summary>
    public string? Brand { get; set; }
    
    /// <summary>
    /// Bus model
    /// </summary>
    public string? Model { get; set; }
    
    /// <summary>
    /// Manufacturing year
    /// </summary>
    public int? YearManufactured { get; set; }
    
    /// <summary>
    /// Total seating capacity
    /// </summary>
    public int Capacity { get; set; }
    
    /// <summary>
    /// Number of seats available (updated on bookings)
    /// </summary>
    public int AvailableSeats { get; set; }
    
    /// <summary>
    /// Type of bus service
    /// </summary>
    public BusType Type { get; set; }
    
    /// <summary>
    /// Current operational status
    /// </summary>
    public BusStatus Status { get; set; } = BusStatus.Active;
    
    /// <summary>
    /// Number of seats in a row (for seat layout)
    /// </summary>
    public int SeatsPerRow { get; set; } = 4;
    
    /// <summary>
    /// Whether the bus has air conditioning
    /// </summary>
    public bool HasAirConditioning { get; set; } = true;
    
    /// <summary>
    /// Whether the bus has WiFi
    /// </summary>
    public bool HasWifi { get; set; } = false;
    
    /// <summary>
    /// Whether the bus is wheelchair accessible
    /// </summary>
    public bool IsWheelchairAccessible { get; set; } = false;
    
    /// <summary>
    /// Current latitude position (for tracking)
    /// </summary>
    public double? CurrentLatitude { get; set; }
    
    /// <summary>
    /// Current longitude position (for tracking)
    /// </summary>
    public double? CurrentLongitude { get; set; }
    
    /// <summary>
    /// Last location update timestamp
    /// </summary>
    public DateTime? LastLocationUpdate { get; set; }
    
    /// <summary>
    /// Driver currently assigned to this bus
    /// </summary>
    public Guid? CurrentDriverId { get; set; }
    public virtual User? CurrentDriver { get; set; }
    
    // Navigation properties
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
