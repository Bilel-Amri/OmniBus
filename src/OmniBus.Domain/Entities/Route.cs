using OmniBus.Domain.Common;

namespace OmniBus.Domain.Entities;

/// <summary>
/// Represents a route between two locations
/// </summary>
public class Route : AuditableEntity
{
    /// <summary>
    /// Route name or code
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Origin city/station
    /// </summary>
    public string Origin { get; set; } = string.Empty;
    
    /// <summary>
    /// Origin city code (e.g., airport code)
    /// </summary>
    public string? OriginCode { get; set; }
    
    /// <summary>
    /// Destination city/station
    /// </summary>
    public string Destination { get; set; } = string.Empty;
    
    /// <summary>
    /// Destination city code
    /// </summary>
    public string? DestinationCode { get; set; }
    
    /// <summary>
    /// Distance in kilometers
    /// </summary>
    public double DistanceKm { get; set; }
    
    /// <summary>
    /// Estimated travel duration in minutes
    /// </summary>
    public int EstimatedDurationMinutes { get; set; }
    
    /// <summary>
    /// Route description
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Whether the route is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Route type (JSON array of stop IDs in order)
    /// </summary>
    public string? RouteStopsJson { get; set; }
    
    /// <summary>
    /// Parsed list of stop names
    /// </summary>
    public List<string> RouteStops => string.IsNullOrEmpty(RouteStopsJson) 
        ? new List<string>() 
        : System.Text.Json.JsonSerializer.Deserialize<List<string>>(RouteStopsJson) ?? new List<string>();
    
    // Navigation properties
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    
    /// <summary>
    /// Intermediate stops for the route
    /// </summary>
    public virtual ICollection<RouteStop> Stops { get; set; } = new List<RouteStop>();
}

/// <summary>
/// Represents an intermediate stop on a route
/// </summary>
public class RouteStop : BaseEntity
{
    /// <summary>
    /// Stop name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Additional notes for finding informal stops (e.g., "In front of the pharmacy", "Main SNTRI station")
    /// Helps passengers locate stops in areas without formal bus stations
    /// </summary>
    public string? StationNote { get; set; }
    
    /// <summary>
    /// Stop order on the route
    /// </summary>
    public int Order { get; set; }
    
    /// <summary>
    /// Distance from origin in km
    /// </summary>
    public double DistanceFromOrigin { get; set; }
    
    /// <summary>
    /// Latitude coordinate
    /// </summary>
    public double? Latitude { get; set; }
    
    /// <summary>
    /// Longitude coordinate
    /// </summary>
    public double? Longitude { get; set; }
    
    /// <summary>
    /// Stop duration in minutes
    /// </summary>
    public int StopDurationMinutes { get; set; } = 2;
    
    /// <summary>
    /// Route this stop belongs to
    /// </summary>
    public Guid RouteId { get; set; }
    public virtual Route Route { get; set; } = null!;
}
