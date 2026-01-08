using AutoMapper;
using OmniBus.Application.DTOs;
using OmniBus.Application.Interfaces;
using OmniBus.Domain.Entities;
using OmniBus.Domain.Enums;
using OmniBus.Domain.Interfaces;

namespace OmniBus.Infrastructure.Services;

/// <summary>
/// Bus service implementation
/// </summary>
public class BusService : IBusService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    
    public BusService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<BusResponseDto>> GetAllBusesAsync()
    {
        var buses = await _unitOfWork.Buses.GetAllAsync();
        return _mapper.Map<IEnumerable<BusResponseDto>>(buses);
    }
    
    public async Task<BusResponseDto?> GetBusByIdAsync(Guid id)
    {
        var bus = await _unitOfWork.Buses.GetByIdAsync(id);
        if (bus == null) return null;
        
        var response = _mapper.Map<BusResponseDto>(bus);
        
        if (bus.CurrentLatitude.HasValue && bus.CurrentLongitude.HasValue)
        {
            response.CurrentLocation = new LocationDto
            {
                Latitude = bus.CurrentLatitude.Value,
                Longitude = bus.CurrentLongitude.Value,
                Timestamp = bus.LastLocationUpdate
            };
        }
        
        return response;
    }
    
    public async Task<BusResponseDto> CreateBusAsync(CreateBusDto request)
    {
        // Check if plate number already exists
        var existingBus = await _unitOfWork.Buses.GetByPlateNumberAsync(request.PlateNumber);
        if (existingBus != null)
        {
            throw new Exception("A bus with this plate number already exists");
        }
        
        var bus = _mapper.Map<Bus>(request);
        bus.AvailableSeats = bus.Capacity;
        bus.Status = BusStatus.Active;
        
        await _unitOfWork.Buses.CreateAsync(bus);
        
        return _mapper.Map<BusResponseDto>(bus);
    }
    
    public async Task<BusResponseDto?> UpdateBusAsync(Guid id, UpdateBusDto request)
    {
        var bus = await _unitOfWork.Buses.GetByIdAsync(id);
        if (bus == null) return null;
        
        // Update fields
        if (!string.IsNullOrEmpty(request.PlateNumber))
            bus.PlateNumber = request.PlateNumber;
        
        if (!string.IsNullOrEmpty(request.RegistrationNumber))
            bus.RegistrationNumber = request.RegistrationNumber;
        
        if (request.Brand != null)
            bus.Brand = request.Brand;
        
        if (request.Model != null)
            bus.Model = request.Model;
        
        if (request.YearManufactured.HasValue)
            bus.YearManufactured = request.YearManufactured.Value;
        
        if (request.Capacity.HasValue)
        {
            var capacityChange = request.Capacity.Value - bus.Capacity;
            bus.Capacity = request.Capacity.Value;
            bus.AvailableSeats += capacityChange;
        }
        
        if (request.Type.HasValue)
            bus.Type = request.Type.Value;
        
        if (request.Status.HasValue)
            bus.Status = request.Status.Value;
        
        if (request.SeatsPerRow.HasValue)
            bus.SeatsPerRow = request.SeatsPerRow.Value;
        
        if (request.HasAirConditioning.HasValue)
            bus.HasAirConditioning = request.HasAirConditioning.Value;
        
        if (request.HasWifi.HasValue)
            bus.HasWifi = request.HasWifi.Value;
        
        if (request.IsWheelchairAccessible.HasValue)
            bus.IsWheelchairAccessible = request.IsWheelchairAccessible.Value;
        
        await _unitOfWork.Buses.UpdateAsync(bus);
        
        return _mapper.Map<BusResponseDto>(bus);
    }
    
    public async Task<bool> DeleteBusAsync(Guid id)
    {
        return await _unitOfWork.Buses.DeleteAsync(id);
    }
    
    public async Task<IEnumerable<BusResponseDto>> GetBusesByStatusAsync(BusStatus status)
    {
        var buses = await _unitOfWork.Buses.GetByStatusAsync(status);
        return _mapper.Map<IEnumerable<BusResponseDto>>(buses);
    }
    
    public async Task<IEnumerable<BusResponseDto>> GetBusesByTypeAsync(BusType type)
    {
        var buses = await _unitOfWork.Buses.GetByTypeAsync(type);
        return _mapper.Map<IEnumerable<BusResponseDto>>(buses);
    }
    
    public async Task<bool> UpdateBusLocationAsync(Guid busId, LocationUpdateDto location)
    {
        await _unitOfWork.Buses.UpdateBusLocationAsync(busId, location.Latitude, location.Longitude);
        return true;
    }
    
    public async Task<IEnumerable<BusResponseDto>> GetActiveBusesWithLocationAsync()
    {
        var buses = await _unitOfWork.Buses.GetByStatusAsync(BusStatus.Active);
        
        return buses.Select(bus =>
        {
            var response = _mapper.Map<BusResponseDto>(bus);
            
            if (bus.CurrentLatitude.HasValue && bus.CurrentLongitude.HasValue)
            {
                response.CurrentLocation = new LocationDto
                {
                    Latitude = bus.CurrentLatitude.Value,
                    Longitude = bus.CurrentLongitude.Value,
                    Timestamp = bus.LastLocationUpdate
                };
            }
            
            return response;
        });
    }
}
