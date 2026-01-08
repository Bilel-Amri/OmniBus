namespace OmniBus.Domain.Interfaces;

/// <summary>
/// Interface for services that need to be notified of domain events
/// </summary>
public interface IDomainEventHandler
{
    Task HandleEventAsync(object domainEvent);
}

/// <summary>
/// Marker interface for domain events
/// </summary>
public interface IDomainEvent
{
    DateTime OccurredAt { get; }
}

/// <summary>
/// Event published when a ticket is booked
/// </summary>
public class TicketBookedEvent : IDomainEvent
{
    public Guid TicketId { get; }
    public Guid ScheduleId { get; }
    public Guid UserId { get; }
    public int SeatNumber { get; }
    public DateTime OccurredAt { get; } = DateTime.UtcNow;

    public TicketBookedEvent(Guid ticketId, Guid scheduleId, Guid userId, int seatNumber)
    {
        TicketId = ticketId;
        ScheduleId = scheduleId;
        UserId = userId;
        SeatNumber = seatNumber;
    }
}

/// <summary>
/// Event published when a bus location is updated
/// </summary>
public class BusLocationUpdatedEvent : IDomainEvent
{
    public Guid ScheduleId { get; }
    public Guid BusId { get; }
    public double Latitude { get; }
    public double Longitude { get; }
    public DateTime OccurredAt { get; } = DateTime.UtcNow;

    public BusLocationUpdatedEvent(Guid scheduleId, Guid busId, double latitude, double longitude)
    {
        ScheduleId = scheduleId;
        BusId = busId;
        Latitude = latitude;
        Longitude = longitude;
    }
}

/// <summary>
/// Event published when a seat is locked for booking
/// </summary>
public class SeatLockedEvent : IDomainEvent
{
    public Guid ScheduleId { get; }
    public int SeatNumber { get; }
    public Guid UserId { get; }
    public DateTime ExpiresAt { get; }
    public DateTime OccurredAt { get; } = DateTime.UtcNow;

    public SeatLockedEvent(Guid scheduleId, int seatNumber, Guid userId, DateTime expiresAt)
    {
        ScheduleId = scheduleId;
        SeatNumber = seatNumber;
        UserId = userId;
        ExpiresAt = expiresAt;
    }
}

/// <summary>
/// Event published when a seat lock is released
/// </summary>
public class SeatLockReleasedEvent : IDomainEvent
{
    public Guid ScheduleId { get; }
    public int SeatNumber { get; }
    public DateTime OccurredAt { get; } = DateTime.UtcNow;

    public SeatLockReleasedEvent(Guid scheduleId, int seatNumber)
    {
        ScheduleId = scheduleId;
        SeatNumber = seatNumber;
    }
}

/// <summary>
/// Event published when a payment is completed
/// </summary>
public class PaymentCompletedEvent : IDomainEvent
{
    public Guid PaymentId { get; }
    public Guid TicketId { get; }
    public decimal Amount { get; }
    public DateTime OccurredAt { get; } = DateTime.UtcNow;

    public PaymentCompletedEvent(Guid paymentId, Guid ticketId, decimal amount)
    {
        PaymentId = paymentId;
        TicketId = ticketId;
        Amount = amount;
    }
}

/// <summary>
/// Event published when a schedule status changes
/// </summary>
public class ScheduleStatusChangedEvent : IDomainEvent
{
    public Guid ScheduleId { get; }
    public Enums.ScheduleStatus OldStatus { get; }
    public Enums.ScheduleStatus NewStatus { get; }
    public string? Reason { get; }
    public DateTime OccurredAt { get; } = DateTime.UtcNow;

    public ScheduleStatusChangedEvent(Guid scheduleId, Enums.ScheduleStatus oldStatus, 
        Enums.ScheduleStatus newStatus, string? reason = null)
    {
        ScheduleId = scheduleId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        Reason = reason;
    }
}
