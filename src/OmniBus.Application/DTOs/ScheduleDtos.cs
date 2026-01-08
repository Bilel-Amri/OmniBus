using OmniBus.Domain.Enums;

namespace OmniBus.Application.DTOs;

public class ScheduleDto : BaseDto
{
    public Guid BusId { get; set; }
    public BusResponseDto? Bus { get; set; }
    public Guid RouteId { get; set; }
    public RouteResponseDto? Route { get; set; }
    public Guid? DriverId { get; set; }
    public string? DriverName { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public DateTime? ActualDepartureTime { get; set; }
    public DateTime? ActualArrivalTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public int AvailableSeats { get; set; }
    public List<int> OperatingDays { get; set; } = new();
    public LocationDto? CurrentLocation { get; set; }
    public string? DelayReason { get; set; }
}

public class ScheduleResponseDto : BaseDto
{
    public BusResponseDto Bus { get; set; } = null!;
    public RouteResponseDto Route { get; set; } = null!;
    public Guid? DriverId { get; set; }
    public string? DriverName { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public DateTime? ActualDepartureTime { get; set; }
    public DateTime? ActualArrivalTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public int AvailableSeats { get; set; }
    public List<int> OperatingDays { get; set; } = new();
    public LocationDto? CurrentLocation { get; set; }
    public string? DelayReason { get; set; }
}

public class CreateScheduleDto
{
    public Guid BusId { get; set; }
    public Guid RouteId { get; set; }
    public Guid? DriverId { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public decimal BasePrice { get; set; }
    public bool IsRecurring { get; set; }
    public List<int> OperatingDays { get; set; } = new();
}

public class UpdateScheduleDto
{
    public Guid? BusId { get; set; }
    public Guid? RouteId { get; set; }
    public Guid? DriverId { get; set; }
    public DateTime? DepartureTime { get; set; }
    public DateTime? ArrivalTime { get; set; }
    public decimal? BasePrice { get; set; }
    public ScheduleStatus? Status { get; set; }
    public string? DelayReason { get; set; }
    public List<int>? OperatingDays { get; set; }
}

public class SearchScheduleDto
{
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string? BusType { get; set; }
    public int? MinSeats { get; set; }
}

public class ScheduleSeatLayoutDto
{
    public Guid ScheduleId { get; set; }
    public DateTime DepartureTime { get; set; }
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public int TotalSeats { get; set; }
    public int SeatsPerRow { get; set; }
    public decimal Price { get; set; }
    public List<SeatStatusDto> Seats { get; set; } = new();
}

public class SeatStatusDto
{
    public int SeatNumber { get; set; }
    public string Status { get; set; } = "Available";
    public bool IsLocked { get; set; }
    public Guid? LockedByUserId { get; set; }
}

public class ScheduleSummaryDto
{
    public Guid Id { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public string BusPlateNumber { get; set; } = string.Empty;
    public string BusType { get; set; } = string.Empty;
}
