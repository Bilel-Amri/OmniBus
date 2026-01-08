using OmniBus.Application.DTOs;
using OmniBus.Application.Interfaces;
using OmniBus.Domain.Entities;
using OmniBus.Domain.Enums;
using OmniBus.Domain.Interfaces;

namespace OmniBus.Infrastructure.Services;

/// <summary>
/// Driver service implementation
/// </summary>
public class DriverService : IDriverService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public DriverService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<bool> UpdateDriverLocationAsync(Guid driverId, LocationUpdateDto location)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(driverId);
        if (user == null || user.Role != UserRole.Driver)
            return false;
        
        // Update user's current location
        // In a real implementation, this would be stored in a separate table or cache
        // For now, we'll update the assigned bus's location
        if (user.AssignedBusId.HasValue)
        {
            await _unitOfWork.Buses.UpdateBusLocationAsync(
                user.AssignedBusId.Value, 
                location.Latitude, 
                location.Longitude);
        }
        
        // Update any active schedules for this driver
        var activeSchedules = await _unitOfWork.Schedules.GetByDriverAsync(driverId);
        var now = DateTime.UtcNow;
        
        foreach (var schedule in activeSchedules)
        {
            // Update schedules that are in progress
            if (schedule.Status == ScheduleStatus.InProgress && schedule.DriverId == driverId)
            {
                await _unitOfWork.Schedules.UpdateScheduleLocationAsync(
                    schedule.Id, 
                    location.Latitude, 
                    location.Longitude);
            }
        }
        
        return true;
    }
    
    public async Task<bool> StartTripAsync(Guid scheduleId, Guid driverId)
    {
        var schedule = await _unitOfWork.Schedules.GetByIdAsync(scheduleId);
        if (schedule == null) return false;
        
        if (schedule.DriverId != driverId)
            return false;
        
        if (schedule.Status != ScheduleStatus.Scheduled && schedule.Status != ScheduleStatus.Delayed)
            return false;
        
        schedule.Status = ScheduleStatus.InProgress;
        schedule.ActualDepartureTime = DateTime.UtcNow;
        
        await _unitOfWork.Schedules.UpdateAsync(schedule);
        await _unitOfWork.SaveChangesAsync();
        
        return true;
    }
    
    public async Task<bool> CompleteTripAsync(Guid scheduleId, Guid driverId)
    {
        var schedule = await _unitOfWork.Schedules.GetByIdAsync(scheduleId);
        if (schedule == null) return false;
        
        if (schedule.DriverId != driverId)
            return false;
        
        if (schedule.Status != ScheduleStatus.InProgress)
            return false;
        
        schedule.Status = ScheduleStatus.Completed;
        schedule.ActualArrivalTime = DateTime.UtcNow;
        
        await _unitOfWork.Schedules.UpdateAsync(schedule);
        await _unitOfWork.SaveChangesAsync();
        
        return true;
    }
    
    public async Task<bool> ReportDelayAsync(Guid scheduleId, Guid driverId, int delayMinutes, string? reason)
    {
        var schedule = await _unitOfWork.Schedules.GetByIdAsync(scheduleId);
        if (schedule == null) return false;
        
        if (schedule.DriverId != driverId)
            return false;
        
        schedule.Status = ScheduleStatus.Delayed;
        schedule.DelayReason = reason ?? $"Delayed by {delayMinutes} minutes";
        
        // Calculate new estimated arrival time
        if (schedule.ArrivalTime > schedule.DepartureTime)
        {
            schedule.ArrivalTime = schedule.ArrivalTime.AddMinutes(delayMinutes);
        }
        
        await _unitOfWork.Schedules.UpdateAsync(schedule);
        await _unitOfWork.SaveChangesAsync();
        
        return true;
    }
}
