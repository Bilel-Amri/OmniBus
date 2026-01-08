using StackExchange.Redis;
using System.Text.Json;

namespace OmniBus.Infrastructure.Services;

/// <summary>
/// Redis-based seat locking service for high-concurrency scenarios
/// Prevents database bloat and ensures sub-millisecond response times
/// </summary>
public interface IRedisLockService
{
    Task<bool> TryLockSeatsAsync(int scheduleId, List<string> seatNumbers, string userId, TimeSpan? expiry = null);
    Task<bool> UnlockSeatsAsync(int scheduleId, List<string> seatNumbers, string userId);
    Task<Dictionary<string, string?>> GetLockedSeatsAsync(int scheduleId);
    Task<bool> RefreshLockAsync(int scheduleId, List<string> seatNumbers, string userId, TimeSpan? expiry = null);
    Task<bool> IsLockedByUserAsync(int scheduleId, string seatNumber, string userId);
}

public class RedisLockService : IRedisLockService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;
    private static readonly TimeSpan DefaultLockDuration = TimeSpan.FromMinutes(10);

    public RedisLockService(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _db = redis.GetDatabase();
    }

    /// <summary>
    /// Attempts to lock seats for a user. Returns true if all seats were successfully locked.
    /// </summary>
    public async Task<bool> TryLockSeatsAsync(int scheduleId, List<string> seatNumbers, string userId, TimeSpan? expiry = null)
    {
        var lockDuration = expiry ?? DefaultLockDuration;
        var lockedSeats = new List<string>();

        try
        {
            foreach (var seatNumber in seatNumbers)
            {
                var key = GetSeatLockKey(scheduleId, seatNumber);
                
                // Try to set the key only if it doesn't exist (NX flag)
                var locked = await _db.StringSetAsync(key, userId, lockDuration, When.NotExists);
                
                if (!locked)
                {
                    // Check if it's already locked by this user (allow re-lock)
                    var existingUserId = await _db.StringGetAsync(key);
                    if (existingUserId != userId)
                    {
                        // Seat is locked by another user - rollback all locks
                        await RollbackLocksAsync(scheduleId, lockedSeats);
                        return false;
                    }
                    
                    // Refresh the lock for this user
                    await _db.StringSetAsync(key, userId, lockDuration);
                }
                
                lockedSeats.Add(seatNumber);
            }

            return true;
        }
        catch
        {
            // On any error, rollback all locks
            await RollbackLocksAsync(scheduleId, lockedSeats);
            throw;
        }
    }

    /// <summary>
    /// Unlocks seats if they are locked by the specified user
    /// </summary>
    public async Task<bool> UnlockSeatsAsync(int scheduleId, List<string> seatNumbers, string userId)
    {
        var allUnlocked = true;

        foreach (var seatNumber in seatNumbers)
        {
            var key = GetSeatLockKey(scheduleId, seatNumber);
            var existingUserId = await _db.StringGetAsync(key);

            // Only unlock if it's locked by this user
            if (existingUserId == userId)
            {
                var deleted = await _db.KeyDeleteAsync(key);
                allUnlocked = allUnlocked && deleted;
            }
        }

        return allUnlocked;
    }

    /// <summary>
    /// Gets all locked seats for a schedule with their user IDs
    /// </summary>
    public async Task<Dictionary<string, string?>> GetLockedSeatsAsync(int scheduleId)
    {
        var pattern = $"seat_lock:{scheduleId}:*";
        var server = _redis.GetServer(_redis.GetEndPoints().First());
        var keys = server.Keys(pattern: pattern);

        var lockedSeats = new Dictionary<string, string?>();

        foreach (var key in keys)
        {
            var seatNumber = key.ToString().Split(':').Last();
            var userId = await _db.StringGetAsync(key);
            lockedSeats[seatNumber] = userId;
        }

        return lockedSeats;
    }

    /// <summary>
    /// Refreshes the lock expiry time for seats locked by the user
    /// </summary>
    public async Task<bool> RefreshLockAsync(int scheduleId, List<string> seatNumbers, string userId, TimeSpan? expiry = null)
    {
        var lockDuration = expiry ?? DefaultLockDuration;
        var allRefreshed = true;

        foreach (var seatNumber in seatNumbers)
        {
            var key = GetSeatLockKey(scheduleId, seatNumber);
            var existingUserId = await _db.StringGetAsync(key);

            if (existingUserId == userId)
            {
                var refreshed = await _db.KeyExpireAsync(key, lockDuration);
                allRefreshed = allRefreshed && refreshed;
            }
            else
            {
                allRefreshed = false;
            }
        }

        return allRefreshed;
    }

    /// <summary>
    /// Checks if a seat is locked by a specific user
    /// </summary>
    public async Task<bool> IsLockedByUserAsync(int scheduleId, string seatNumber, string userId)
    {
        var key = GetSeatLockKey(scheduleId, seatNumber);
        var existingUserId = await _db.StringGetAsync(key);
        return existingUserId == userId;
    }

    private static string GetSeatLockKey(int scheduleId, string seatNumber)
    {
        return $"seat_lock:{scheduleId}:{seatNumber}";
    }

    private async Task RollbackLocksAsync(int scheduleId, List<string> seatNumbers)
    {
        foreach (var seatNumber in seatNumbers)
        {
            var key = GetSeatLockKey(scheduleId, seatNumber);
            await _db.KeyDeleteAsync(key);
        }
    }
}
