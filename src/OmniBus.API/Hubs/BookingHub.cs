using Microsoft.AspNetCore.SignalR;
using OmniBus.Domain.Enums;

namespace OmniBus.API.Hubs;

/// <summary>
/// SignalR hub for real-time seat booking updates
/// </summary>
public class BookingHub : Hub
{
    private readonly IHubContext<BookingHub> _hubContext;
    
    public BookingHub(IHubContext<BookingHub> hubContext)
    {
        _hubContext = hubContext;
    }
    
    /// <summary>
    /// Join a schedule group to receive seat availability updates
    /// </summary>
    public async Task JoinSchedule(Guid scheduleId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"booking_{scheduleId}");
        await Clients.Caller.SendAsync("JoinedSchedule", scheduleId);
    }
    
    /// <summary>
    /// Leave schedule group
    /// </summary>
    public async Task LeaveSchedule(Guid scheduleId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"booking_{scheduleId}");
    }
    
    /// <summary>
    /// Broadcast when a seat is locked for booking
    /// </summary>
    public async Task BroadcastSeatLocked(Guid scheduleId, int seatNumber, Guid userId, DateTime expiresAt)
    {
        await _hubContext.Clients.Group($"booking_{scheduleId}").SendAsync("SeatLocked", new
        {
            ScheduleId = scheduleId,
            SeatNumber = seatNumber,
            UserId = userId,
            ExpiresAt = expiresAt
        });
    }
    
    /// <summary>
    /// Broadcast when a seat lock is released
    /// </summary>
    public async Task BroadcastSeatLockReleased(Guid scheduleId, int seatNumber)
    {
        await _hubContext.Clients.Group($"booking_{scheduleId}").SendAsync("SeatLockReleased", new
        {
            ScheduleId = scheduleId,
            SeatNumber = seatNumber
        });
    }
    
    /// <summary>
    /// Broadcast when a seat is successfully booked
    /// </summary>
    public async Task BroadcastSeatBooked(Guid scheduleId, int seatNumber, Guid userId, string bookingReference)
    {
        await _hubContext.Clients.Group($"booking_{scheduleId}").SendAsync("SeatBooked", new
        {
            ScheduleId = scheduleId,
            SeatNumber = seatNumber,
            UserId = userId,
            BookingReference = bookingReference
        });
    }
    
    /// <summary>
    /// Broadcast when seat availability changes
    /// </summary>
    public async Task BroadcastAvailabilityChange(Guid scheduleId, int availableSeats)
    {
        await _hubContext.Clients.Group($"booking_{scheduleId}").SendAsync("AvailabilityChanged", new
        {
            ScheduleId = scheduleId,
            AvailableSeats = availableSeats
        });
    }
    
    /// <summary>
    /// Broadcast ticket confirmation
    /// </summary>
    public async Task BroadcastTicketConfirmed(Guid scheduleId, string bookingReference, string passengerName, int seatNumber)
    {
        await _hubContext.Clients.Group($"booking_{scheduleId}").SendAsync("TicketConfirmed", new
        {
            ScheduleId = scheduleId,
            BookingReference = bookingReference,
            PassengerName = passengerName,
            SeatNumber = seatNumber
        });
    }
    
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        Console.WriteLine($"Booking client connected: {Context.ConnectionId}");
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
        Console.WriteLine($"Booking client disconnected: {Context.ConnectionId}");
    }
}
