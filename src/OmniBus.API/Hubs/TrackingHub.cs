using Microsoft.AspNetCore.SignalR;
using OmniBus.Application.DTOs;

namespace OmniBus.API.Hubs;

/// <summary>
/// SignalR hub for real-time bus tracking
/// </summary>
public class TrackingHub : Hub
{
    private readonly IHubContext<TrackingHub> _hubContext;
    
    public TrackingHub(IHubContext<TrackingHub> hubContext)
    {
        _hubContext = hubContext;
    }
    
    /// <summary>
    /// Join a route group to receive updates for that route
    /// </summary>
    public async Task JoinRoute(Guid routeId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"route_{routeId}");
        await Clients.Caller.SendAsync("JoinedRoute", routeId);
    }
    
    /// <summary>
    /// Leave a route group
    /// </summary>
    public async Task LeaveRoute(Guid routeId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"route_{routeId}");
    }
    
    /// <summary>
    /// Join schedule group for specific schedule updates
    /// </summary>
    public async Task JoinSchedule(Guid scheduleId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"schedule_{scheduleId}");
        await Clients.Caller.SendAsync("JoinedSchedule", scheduleId);
    }
    
    /// <summary>
    /// Leave schedule group
    /// </summary>
    public async Task LeaveSchedule(Guid scheduleId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"schedule_{scheduleId}");
    }
    
    /// <summary>
    /// Join admin group for monitoring all buses
    /// </summary>
    public async Task JoinAdmin()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "admin");
        await Clients.Caller.SendAsync("JoinedAdmin");
    }
    
    /// <summary>
    /// Leave admin group
    /// </summary>
    public async Task LeaveAdmin()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "admin");
    }
    
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        Console.WriteLine($"Client connected: {Context.ConnectionId}");
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // Clean up group memberships on disconnect
        await base.OnDisconnectedAsync(exception);
        Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
    }
    
    /// <summary>
    /// Broadcast bus location to all clients monitoring a route
    /// </summary>
    public async Task BroadcastBusLocation(Guid scheduleId, Guid routeId, double latitude, double longitude, DateTime timestamp)
    {
        var locationData = new LocationDto
        {
            Latitude = latitude,
            Longitude = longitude,
            Timestamp = timestamp
        };
        
        // Send to route group
        await _hubContext.Clients.Group($"route_{routeId}").SendAsync("BusLocationUpdated", scheduleId, locationData);
        
        // Send to schedule group
        await _hubContext.Clients.Group($"schedule_{scheduleId}").SendAsync("BusLocationUpdated", scheduleId, locationData);
        
        // Send to admin group
        await _hubContext.Clients.Group("admin").SendAsync("BusLocationUpdated", scheduleId, locationData);
    }
    
    /// <summary>
    /// Broadcast schedule status change
    /// </summary>
    public async Task BroadcastScheduleStatusChange(Guid scheduleId, string status, string? reason)
    {
        await _hubContext.Clients.Group($"schedule_{scheduleId}").SendAsync("ScheduleStatusChanged", scheduleId, status, reason);
        await _hubContext.Clients.Group("admin").SendAsync("ScheduleStatusChanged", scheduleId, status, reason);
    }
    
    /// <summary>
    /// Broadcast delay notification
    /// </summary>
    public async Task BroadcastDelayNotification(Guid scheduleId, int delayMinutes, string reason)
    {
        var delayInfo = new
        {
            ScheduleId = scheduleId,
            DelayMinutes = delayMinutes,
            Reason = reason,
            NewDepartureTime = DateTime.UtcNow.AddMinutes(delayMinutes)
        };
        
        await _hubContext.Clients.Group($"schedule_{scheduleId}").SendAsync("DelayNotification", delayInfo);
        await _hubContext.Clients.Group("admin").SendAsync("DelayNotification", delayInfo);
    }
}
