namespace OmniBus.Application.DTOs;

public class TicketDto : BaseDto
{
    public Guid UserId { get; set; }
    public Guid ScheduleId { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public int SeatNumber { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime BookingDate { get; set; }
    public string? QrCode { get; set; }
    public string? PassengerName { get; set; }
    public string? PassengerPhone { get; set; }
    public ScheduleSummaryDto? Schedule { get; set; }
}

public class TicketResponseDto : BaseDto
{
    public Guid UserId { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public int SeatNumber { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime BookingDate { get; set; }
    public string? QrCode { get; set; }
    public string? PassengerName { get; set; }
    public ScheduleSummaryDto? Schedule { get; set; }
}

public class MyTicketDto : BaseDto
{
    public string BookingReference { get; set; } = string.Empty;
    public int SeatNumber { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime BookingDate { get; set; }
    public string? QrCode { get; set; }
    public DateTime DepartureTime { get; set; }
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public string BusType { get; set; } = string.Empty;
    public string? CancellationReason { get; set; }
}

public class CreateTicketDto
{
    public Guid ScheduleId { get; set; }
    public int SeatNumber { get; set; }
    public string? PassengerName { get; set; }
    public string? PassengerPhone { get; set; }
}

public class CancelTicketDto
{
    public string Reason { get; set; } = string.Empty;
}

public class SeatLockRequestDto
{
    public Guid ScheduleId { get; set; }
    public int SeatNumber { get; set; }
}

public class SeatLockResponseDto
{
    public Guid LockId { get; set; }
    public Guid ScheduleId { get; set; }
    public int SeatNumber { get; set; }
    public DateTime ExpiresAt { get; set; }
    public int DurationSeconds { get; set; }
}

public class TicketStatsDto
{
    public int TotalTickets { get; set; }
    public int CompletedTickets { get; set; }
    public int CancelledTickets { get; set; }
    public int PendingTickets { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TodayRevenue { get; set; }
}

public class ConfirmBoardingDto
{
    public Guid TicketId { get; set; }
    public string? QrCode { get; set; }
}
