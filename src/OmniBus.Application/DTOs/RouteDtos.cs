namespace OmniBus.Application.DTOs;

public class RouteDto : BaseDto
{
    public string Name { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string? OriginCode { get; set; }
    public string Destination { get; set; } = string.Empty;
    public string? DestinationCode { get; set; }
    public double DistanceKm { get; set; }
    public int EstimatedDurationMinutes { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public List<RouteStopDto>? Stops { get; set; }
}

public class RouteResponseDto : BaseDto
{
    public string Name { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string? OriginCode { get; set; }
    public string Destination { get; set; } = string.Empty;
    public string? DestinationCode { get; set; }
    public double DistanceKm { get; set; }
    public int EstimatedDurationMinutes { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public List<RouteStopDto>? Stops { get; set; }
}

public class CreateRouteDto
{
    public string Name { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string? OriginCode { get; set; }
    public string Destination { get; set; } = string.Empty;
    public string? DestinationCode { get; set; }
    public double DistanceKm { get; set; }
    public int EstimatedDurationMinutes { get; set; }
    public string? Description { get; set; }
    public List<CreateRouteStopDto>? Stops { get; set; }
}

public class UpdateRouteDto
{
    public string? Name { get; set; }
    public string? Origin { get; set; }
    public string? OriginCode { get; set; }
    public string? Destination { get; set; }
    public string? DestinationCode { get; set; }
    public double? DistanceKm { get; set; }
    public int? EstimatedDurationMinutes { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
}

public class RouteStopDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
    public double DistanceFromOrigin { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int StopDurationMinutes { get; set; }
}

public class CreateRouteStopDto
{
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
    public double DistanceFromOrigin { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int StopDurationMinutes { get; set; }
}
