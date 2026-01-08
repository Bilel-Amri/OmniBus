using OmniBus.Domain.Enums;

namespace OmniBus.Application.DTOs;

public class BusDto : BaseDto
{
    public string PlateNumber { get; set; } = string.Empty;
    public string? RegistrationNumber { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public int? YearManufactured { get; set; }
    public int Capacity { get; set; }
    public int AvailableSeats { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int SeatsPerRow { get; set; }
    public bool HasAirConditioning { get; set; }
    public bool HasWifi { get; set; }
    public bool IsWheelchairAccessible { get; set; }
    public LocationDto? CurrentLocation { get; set; }
}

public class BusResponseDto : BaseDto
{
    public string PlateNumber { get; set; } = string.Empty;
    public string? RegistrationNumber { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public int? YearManufactured { get; set; }
    public int Capacity { get; set; }
    public int AvailableSeats { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int SeatsPerRow { get; set; }
    public bool HasAirConditioning { get; set; }
    public bool HasWifi { get; set; }
    public bool IsWheelchairAccessible { get; set; }
    public LocationDto? CurrentLocation { get; set; }
}

public class CreateBusDto
{
    public string PlateNumber { get; set; } = string.Empty;
    public string? RegistrationNumber { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public int? YearManufactured { get; set; }
    public int Capacity { get; set; }
    public BusType Type { get; set; }
    public int SeatsPerRow { get; set; } = 4;
    public bool HasAirConditioning { get; set; } = true;
    public bool HasWifi { get; set; } = false;
    public bool IsWheelchairAccessible { get; set; } = false;
}

public class UpdateBusDto
{
    public string? PlateNumber { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public int? YearManufactured { get; set; }
    public int? Capacity { get; set; }
    public BusType? Type { get; set; }
    public BusStatus? Status { get; set; }
    public int? SeatsPerRow { get; set; }
    public bool? HasAirConditioning { get; set; }
    public bool? HasWifi { get; set; }
    public bool? IsWheelchairAccessible { get; set; }
}

public class LocationUpdateDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime? Timestamp { get; set; }
}

public class LocationDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime? Timestamp { get; set; }
}

public class BusLocationDto
{
    public Guid BusId { get; set; }
    public string PlateNumber { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime? LastUpdate { get; set; }
}
