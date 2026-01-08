using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace OmniBus.API.Hubs;

/// <summary>
/// Real-time GPS tracking hub with Smart Throttling
/// Broadcasts every 10s when moving (>10 km/h), 60s when stationary to preserve battery
/// </summary>
public class SmartTrackingHub : Hub
{
    // Store last known position and speed for each bus
    private static readonly ConcurrentDictionary<int, BusTrackingState> BusStates = new();
    private const double MOVING_SPEED_THRESHOLD = 10.0; // km/h
    private const int MOVING_UPDATE_INTERVAL = 10; // seconds
    private const int STATIONARY_UPDATE_INTERVAL = 60; // seconds

    /// <summary>
    /// Update bus location with smart throttling
    /// </summary>
    public async Task UpdateBusLocation(int busId, int scheduleId, double latitude, double longitude, double speed, string? status = null)
    {
        var now = DateTime.UtcNow;
        var state = BusStates.GetOrAdd(busId, _ => new BusTrackingState());

        // Calculate if bus is moving based on speed threshold
        var isMoving = speed > MOVING_SPEED_THRESHOLD;
        var requiredInterval = isMoving ? MOVING_UPDATE_INTERVAL : STATIONARY_UPDATE_INTERVAL;

        // Check if enough time has passed since last update
        var timeSinceLastUpdate = (now - state.LastUpdateTime).TotalSeconds;
        if (timeSinceLastUpdate < requiredInterval && state.HasInitialUpdate)
        {
            // Throttle: Skip this update to save battery
            await Clients.Caller.SendAsync("UpdateThrottled", new
            {
                busId,
                reason = $"Throttled. Next update in {requiredInterval - (int)timeSinceLastUpdate}s",
                interval = requiredInterval
            });
            return;
        }

        // Update state
        state.Latitude = latitude;
        state.Longitude = longitude;
        state.Speed = speed;
        state.LastUpdateTime = now;
        state.HasInitialUpdate = true;
        state.IsMoving = isMoving;

        // Broadcast to all connected clients subscribed to this bus
        var locationUpdate = new
        {
            busId,
            scheduleId,
            latitude,
            longitude,
            speed,
            status = status ?? (isMoving ? "En route" : "Arrêté"),
            timestamp = now,
            updateInterval = requiredInterval,
            isMoving
        };

        await Clients.Group($"bus_{busId}").SendAsync("BusLocationUpdated", locationUpdate);
        await Clients.Group($"schedule_{scheduleId}").SendAsync("BusLocationUpdated", locationUpdate);
        await Clients.Group("admin").SendAsync("BusLocationUpdated", locationUpdate);
    }

    /// <summary>
    /// Subscribe to bus updates
    /// </summary>
    public async Task SubscribeToBus(int busId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"bus_{busId}");
        
        // Send current state if available
        if (BusStates.TryGetValue(busId, out var state) && state.HasInitialUpdate)
        {
            await Clients.Caller.SendAsync("CurrentBusLocation", new
            {
                busId,
                latitude = state.Latitude,
                longitude = state.Longitude,
                speed = state.Speed,
                lastUpdate = state.LastUpdateTime,
                isMoving = state.IsMoving
            });
        }
    }

    /// <summary>
    /// Unsubscribe from bus updates
    /// </summary>
    public async Task UnsubscribeFromBus(int busId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"bus_{busId}");
    }

    /// <summary>
    /// Subscribe to schedule updates
    /// </summary>
    public async Task SubscribeToSchedule(int scheduleId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"schedule_{scheduleId}");
    }

    /// <summary>
    /// Unsubscribe from schedule updates
    /// </summary>
    public async Task UnsubscribeFromSchedule(int scheduleId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"schedule_{scheduleId}");
    }

    /// <summary>
    /// Subscribe to admin monitoring (all buses)
    /// </summary>
    public async Task SubscribeAdmin()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "admin");
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        await Clients.Caller.SendAsync("Connected", Context.ConnectionId);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}

/// <summary>
/// Tracking state for each bus
/// </summary>
public class BusTrackingState
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Speed { get; set; }
    public DateTime LastUpdateTime { get; set; } = DateTime.MinValue;
    public bool HasInitialUpdate { get; set; }
    public bool IsMoving { get; set; }
}
