using OmniBus.Domain.Common;
using OmniBus.Domain.Enums;

namespace OmniBus.Domain.Entities;

/// <summary>
/// Represents a scheduled trip
/// </summary>
public class Schedule : AuditableEntity
{
    /// <summary>
    /// Bus assigned to this schedule
    /// </summary>
    public Guid BusId { get; set; }
    public virtual Bus Bus { get; set; } = null!;
    
    /// <summary>
    /// Route for this schedule
    /// </summary>
    public Guid RouteId { get; set; }
    public virtual Route Route { get; set; } = null!;
    
    /// <summary>
    /// Driver assigned to this schedule
    /// </summary>
    public Guid? DriverId { get; set; }
    public virtual User? Driver { get; set; }
    
    /// <summary>
    /// Scheduled departure time
    /// </summary>
    public DateTime DepartureTime { get; set; }
    
    /// <summary>
    /// Scheduled arrival time
    /// </summary>
    public DateTime ArrivalTime { get; set; }
    
    /// <summary>
    /// Actual departure time (when bus leaves)
    /// </summary>
    public DateTime? ActualDepartureTime { get; set; }
    
    /// <summary>
    /// Actual arrival time (when bus arrives)
    /// </summary>
    public DateTime? ActualArrivalTime { get; set; }
    
    /// <summary>
    /// Current status of the trip
    /// </summary>
    public ScheduleStatus Status { get; set; } = ScheduleStatus.Scheduled;
    
    /// <summary>
    /// Base price for this trip
    /// </summary>
    public decimal BasePrice { get; set; }
    
    /// <summary>
    /// Current available seats
    /// </summary>
    public int AvailableSeats { get; set; }
    
    /// <summary>
    /// Days of week this schedule runs (JSON array)
    /// </summary>
    public string? OperatingDaysJson { get; set; }
    
    /// <summary>
    /// Parse operating days
    /// </summary>
    public List<int> OperatingDays => string.IsNullOrEmpty(OperatingDaysJson)
        ? new List<int> { 0, 1, 2, 3, 4, 5, 6 }
        : System.Text.Json.JsonSerializer.Deserialize<List<int>>(OperatingDaysJson) ?? new List<int>();
    
    /// <summary>
    /// Whether this is a recurring schedule
    /// </summary>
    public bool IsRecurring { get; set; } = false;
    
    /// <summary>
    /// Delay reason if status is Delayed
    /// </summary>
    public string? DelayReason { get; set; }
    
    /// <summary>
    /// Current latitude of the bus (for tracking)
    /// </summary>
    public double? CurrentLatitude { get; set; }
    
    /// <summary>
    /// Current longitude of the bus (for tracking)
    /// </summary>
    public double? CurrentLongitude { get; set; }
    
    /// <summary>
    /// Last tracking update
    /// </summary>
    public DateTime? LastTrackingUpdate { get; set; }
    
    // Navigation properties
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    
    /// <summary>
    /// Seat locks for concurrent booking
    /// </summary>
    public virtual ICollection<SeatLock> SeatLocks { get; set; } = new List<SeatLock>();
}
