namespace OmniBus.Application.DTOs;

public class SeatDto : BaseDto
{
    public int BusId { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public string SeatType { get; set; } = "Regular";
    public bool IsAvailable { get; set; } = true;
    public decimal ExtraCharge { get; set; }
}

public class SeatLayoutDto
{
    public int BusId { get; set; }
    public int TotalSeats { get; set; }
    public int SeatsPerRow { get; set; }
    public List<SeatRowDto> Rows { get; set; } = new();
}

public class SeatRowDto
{
    public int RowNumber { get; set; }
    public List<SeatDto> Seats { get; set; } = new();
}

public class SeatLockDto : BaseDto
{
    public Guid ScheduleId { get; set; }
    public Guid UserId { get; set; }
    public int SeatNumber { get; set; }
    public DateTime LockedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? SessionId { get; set; }
}
