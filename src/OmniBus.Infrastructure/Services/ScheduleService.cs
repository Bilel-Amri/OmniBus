using AutoMapper;
using OmniBus.Application.DTOs;
using OmniBus.Application.Interfaces;
using OmniBus.Domain.Entities;
using OmniBus.Domain.Enums;
using OmniBus.Domain.Interfaces;

namespace OmniBus.Infrastructure.Services;

/// <summary>
/// Schedule service implementation
/// </summary>
public class ScheduleService : IScheduleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    
    public ScheduleService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<ScheduleResponseDto>> GetAllSchedulesAsync()
    {
        var schedules = await _unitOfWork.Schedules.GetAllAsync();
        return schedules.Select(MapToResponse).ToList();
    }
    
    public async Task<ScheduleResponseDto?> GetScheduleByIdAsync(Guid id)
    {
        var schedule = await _unitOfWork.Schedules.GetByIdAsync(id);
        if (schedule == null) return null;
        return MapToResponse(schedule);
    }
    
    public async Task<ScheduleResponseDto> CreateScheduleAsync(CreateScheduleDto request)
    {
        var bus = await _unitOfWork.Buses.GetByIdAsync(request.BusId);
        if (bus == null)
            throw new Exception("Bus not found");
        
        var route = await _unitOfWork.Routes.GetByIdAsync(request.RouteId);
        if (route == null)
            throw new Exception("Route not found");
        
        var schedule = new Schedule
        {
            BusId = request.BusId,
            RouteId = request.RouteId,
            DriverId = request.DriverId,
            DepartureTime = request.DepartureTime,
            ArrivalTime = request.ArrivalTime,
            BasePrice = request.BasePrice,
            AvailableSeats = bus.Capacity,
            IsRecurring = request.IsRecurring,
            OperatingDaysJson = request.OperatingDays.Any() ? 
                System.Text.Json.JsonSerializer.Serialize(request.OperatingDays) : null
        };
        
        await _unitOfWork.Schedules.CreateAsync(schedule);
        
        return MapToResponse(schedule);
    }
    
    public async Task<ScheduleResponseDto?> UpdateScheduleAsync(Guid id, UpdateScheduleDto request)
    {
        var schedule = await _unitOfWork.Schedules.GetByIdAsync(id);
        if (schedule == null) return null;
        
        if (request.BusId.HasValue)
            schedule.BusId = request.BusId.Value;
        
        if (request.RouteId.HasValue)
            schedule.RouteId = request.RouteId.Value;
        
        if (request.DriverId.HasValue)
            schedule.DriverId = request.DriverId.Value;
        
        if (request.DepartureTime.HasValue)
            schedule.DepartureTime = request.DepartureTime.Value;
        
        if (request.ArrivalTime.HasValue)
            schedule.ArrivalTime = request.ArrivalTime.Value;
        
        if (request.BasePrice.HasValue)
            schedule.BasePrice = request.BasePrice.Value;
        
        if (request.Status.HasValue)
            schedule.Status = request.Status.Value;
        
        if (request.DelayReason != null)
            schedule.DelayReason = request.DelayReason;
        
        if (request.OperatingDays != null)
            schedule.OperatingDaysJson = System.Text.Json.JsonSerializer.Serialize(request.OperatingDays);
        
        await _unitOfWork.Schedules.UpdateAsync(schedule);
        
        // Reload with navigation properties
        schedule = await _unitOfWork.Schedules.GetByIdAsync(id);
        return MapToResponse(schedule);
    }
    
    public async Task<bool> DeleteScheduleAsync(Guid id)
    {
        return await _unitOfWork.Schedules.DeleteAsync(id);
    }
    
    public async Task<IEnumerable<ScheduleResponseDto>> SearchSchedulesAsync(SearchScheduleDto request)
    {
        // Ensure date is UTC for PostgreSQL compatibility
        var utcDate = request.Date.Kind == DateTimeKind.Unspecified 
            ? DateTime.SpecifyKind(request.Date, DateTimeKind.Utc) 
            : request.Date.ToUniversalTime();
            
        var schedules = await _unitOfWork.Schedules.SearchSchedulesAsync(
            request.Origin, request.Destination, utcDate);
        return schedules.Select(MapToResponse).ToList();
    }
    
    public async Task<IEnumerable<ScheduleResponseDto>> GetSchedulesByDateAsync(DateTime date)
    {
        // Ensure date is UTC for PostgreSQL compatibility
        var utcDate = date.Kind == DateTimeKind.Unspecified 
            ? DateTime.SpecifyKind(date, DateTimeKind.Utc) 
            : date.ToUniversalTime();
            
        var schedules = await _unitOfWork.Schedules.GetSchedulesByDateAsync(utcDate);
        return schedules.Select(MapToResponse).ToList();
    }
    
    public async Task<IEnumerable<ScheduleResponseDto>> GetSchedulesByDriverAsync(Guid driverId)
    {
        var schedules = await _unitOfWork.Schedules.GetByDriverAsync(driverId);
        return schedules.Select(MapToResponse).ToList();
    }
    
    public async Task<IEnumerable<ScheduleResponseDto>> GetSchedulesByRouteAsync(Guid routeId)
    {
        var schedules = await _unitOfWork.Schedules.GetByRouteAsync(routeId);
        return schedules.Select(MapToResponse).ToList();
    }
    
    public async Task<ScheduleSeatLayoutDto?> GetSeatLayoutAsync(Guid scheduleId)
    {
        var schedule = await _unitOfWork.Schedules.GetByIdAsync(scheduleId);
        if (schedule == null) return null;
        
        var layout = new ScheduleSeatLayoutDto
        {
            ScheduleId = schedule.Id,
            DepartureTime = schedule.DepartureTime,
            Origin = schedule.Route.Origin,
            Destination = schedule.Route.Destination,
            TotalSeats = schedule.Bus.Capacity,
            SeatsPerRow = schedule.Bus.SeatsPerRow,
            Price = schedule.BasePrice
        };
        
        // Get booked seats
        var bookedSeats = await _unitOfWork.Tickets.GetBookedSeatsAsync(scheduleId);
        var bookedSeatsSet = bookedSeats.ToHashSet();
        
        // Get locked seats
        var lockedLocks = await _unitOfWork.SeatLocks.GetScheduleLocksAsync(scheduleId);
        var lockedSeats = lockedLocks.Where(l => l.IsValid).Select(l => l.SeatNumber).ToHashSet();
        
        // Generate seat statuses
        for (int seat = 1; seat <= schedule.Bus.Capacity; seat++)
        {
            var status = new SeatStatusDto { SeatNumber = seat };
            
            if (bookedSeatsSet.Contains(seat))
                status.Status = "Booked";
            else if (lockedSeats.Contains(seat))
                status.Status = "Locked";
            else
                status.Status = "Available";
            
            layout.Seats.Add(status);
        }
        
        return layout;
    }
    
    public async Task<IEnumerable<ScheduleResponseDto>> GetActiveSchedulesWithLocationAsync()
    {
        var now = DateTime.UtcNow;
        var schedules = await _unitOfWork.Schedules.GetUpcomingSchedulesAsync(now);
        
        return schedules.Select(schedule =>
        {
            var response = MapToResponse(schedule);
            
            if (schedule.CurrentLatitude.HasValue && schedule.CurrentLongitude.HasValue)
            {
                response.CurrentLocation = new LocationDto
                {
                    Latitude = schedule.CurrentLatitude.Value,
                    Longitude = schedule.CurrentLongitude.Value,
                    Timestamp = schedule.LastTrackingUpdate
                };
            }
            
            return response;
        }).ToList();
    }
    
    public async Task<bool> UpdateScheduleStatusAsync(Guid id, ScheduleStatus status, string? reason = null)
    {
        var schedule = await _unitOfWork.Schedules.GetByIdAsync(id);
        if (schedule == null) return false;
        
        schedule.Status = status;
        if (reason != null)
            schedule.DelayReason = reason;
        
        if (status == ScheduleStatus.InProgress && !schedule.ActualDepartureTime.HasValue)
            schedule.ActualDepartureTime = DateTime.UtcNow;
        
        if (status == ScheduleStatus.Completed && !schedule.ActualArrivalTime.HasValue)
            schedule.ActualArrivalTime = DateTime.UtcNow;
        
        await _unitOfWork.Schedules.UpdateAsync(schedule);
        
        return true;
    }
    
    public async Task<bool> UpdateScheduleLocationAsync(Guid scheduleId, LocationUpdateDto location)
    {
        await _unitOfWork.Schedules.UpdateScheduleLocationAsync(
            scheduleId, location.Latitude, location.Longitude);
        return true;
    }
    
    private ScheduleResponseDto MapToResponse(Schedule schedule)
    {
        return new ScheduleResponseDto
        {
            Id = schedule.Id,
            Bus = new BusResponseDto
            {
                Id = schedule.Bus.Id,
                PlateNumber = schedule.Bus.PlateNumber,
                Capacity = schedule.Bus.Capacity,
                AvailableSeats = schedule.AvailableSeats,
                Type = schedule.Bus.Type.ToString(),
                Status = schedule.Bus.Status.ToString(),
                HasAirConditioning = schedule.Bus.HasAirConditioning,
                HasWifi = schedule.Bus.HasWifi,
                IsWheelchairAccessible = schedule.Bus.IsWheelchairAccessible
            },
            Route = new RouteResponseDto
            {
                Id = schedule.Route.Id,
                Name = schedule.Route.Name,
                Origin = schedule.Route.Origin,
                OriginCode = schedule.Route.OriginCode,
                Destination = schedule.Route.Destination,
                DestinationCode = schedule.Route.DestinationCode,
                DistanceKm = schedule.Route.DistanceKm,
                EstimatedDurationMinutes = schedule.Route.EstimatedDurationMinutes,
                IsActive = schedule.Route.IsActive
            },
            DriverId = schedule.DriverId,
            DriverName = schedule.Driver != null ? $"{schedule.Driver.FirstName} {schedule.Driver.LastName}" : null,
            DepartureTime = schedule.DepartureTime,
            ArrivalTime = schedule.ArrivalTime,
            ActualDepartureTime = schedule.ActualDepartureTime,
            ActualArrivalTime = schedule.ActualArrivalTime,
            Status = schedule.Status.ToString(),
            BasePrice = schedule.BasePrice,
            AvailableSeats = schedule.AvailableSeats,
            OperatingDays = schedule.OperatingDays,
            CurrentLocation = schedule.CurrentLatitude.HasValue && schedule.CurrentLongitude.HasValue ?
                new LocationDto
                {
                    Latitude = schedule.CurrentLatitude.Value,
                    Longitude = schedule.CurrentLongitude.Value,
                    Timestamp = schedule.LastTrackingUpdate
                } : null,
            DelayReason = schedule.DelayReason
        };
    }
}
