using OmniBus.Application.DTOs;

namespace OmniBus.Application.Interfaces;

public interface IDriverService
{
    Task<bool> UpdateDriverLocationAsync(Guid driverId, LocationUpdateDto location);
    Task<bool> StartTripAsync(Guid scheduleId, Guid driverId);
    Task<bool> CompleteTripAsync(Guid scheduleId, Guid driverId);
    Task<bool> ReportDelayAsync(Guid scheduleId, Guid driverId, int delayMinutes, string? reason);
}
