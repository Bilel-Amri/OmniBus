using AutoMapper;
using OmniBus.Application.DTOs;
using OmniBus.Application.Interfaces;
using OmniBus.Domain.Entities;
using OmniBus.Domain.Interfaces;

namespace OmniBus.Infrastructure.Services;

/// <summary>
/// Route service implementation
/// </summary>
public class RouteService : IRouteService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    
    public RouteService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<RouteResponseDto>> GetAllRoutesAsync()
    {
        var routes = await _unitOfWork.Routes.GetAllAsync();
        return _mapper.Map<IEnumerable<RouteResponseDto>>(routes);
    }
    
    public async Task<RouteResponseDto?> GetRouteByIdAsync(Guid id)
    {
        var route = await _unitOfWork.Routes.GetByIdAsync(id);
        if (route == null) return null;
        return _mapper.Map<RouteResponseDto>(route);
    }
    
    public async Task<RouteResponseDto> CreateRouteAsync(CreateRouteDto request)
    {
        var route = _mapper.Map<Route>(request);
        
        // Handle stops
        if (request.Stops != null && request.Stops.Any())
        {
            var stops = request.Stops.Select((stop, index) => new RouteStop
            {
                Name = stop.Name,
                Order = stop.Order,
                DistanceFromOrigin = stop.DistanceFromOrigin,
                Latitude = stop.Latitude,
                Longitude = stop.Longitude,
                StopDurationMinutes = stop.StopDurationMinutes
            }).ToList();
            
            route.Stops = stops;
        }
        
        await _unitOfWork.Routes.CreateAsync(route);
        
        return _mapper.Map<RouteResponseDto>(route);
    }
    
    public async Task<RouteResponseDto?> UpdateRouteAsync(Guid id, UpdateRouteDto request)
    {
        var route = await _unitOfWork.Routes.GetByIdAsync(id);
        if (route == null) return null;
        
        // Update fields
        if (!string.IsNullOrEmpty(request.Name))
            route.Name = request.Name;
        
        if (!string.IsNullOrEmpty(request.Origin))
            route.Origin = request.Origin;
        
        if (request.OriginCode != null)
            route.OriginCode = request.OriginCode;
        
        if (!string.IsNullOrEmpty(request.Destination))
            route.Destination = request.Destination;
        
        if (request.DestinationCode != null)
            route.DestinationCode = request.DestinationCode;
        
        if (request.DistanceKm.HasValue)
            route.DistanceKm = request.DistanceKm.Value;
        
        if (request.EstimatedDurationMinutes.HasValue)
            route.EstimatedDurationMinutes = request.EstimatedDurationMinutes.Value;
        
        if (request.Description != null)
            route.Description = request.Description;
        
        if (request.IsActive.HasValue)
            route.IsActive = request.IsActive.Value;
        
        await _unitOfWork.Routes.UpdateAsync(route);
        
        // Reload with stops
        route = await _unitOfWork.Routes.GetByIdAsync(id);
        return _mapper.Map<RouteResponseDto>(route);
    }
    
    public async Task<bool> DeleteRouteAsync(Guid id)
    {
        return await _unitOfWork.Routes.DeleteAsync(id);
    }
    
    public async Task<IEnumerable<RouteResponseDto>> SearchRoutesAsync(string origin, string destination)
    {
        var routes = await _unitOfWork.Routes.SearchRoutesAsync(origin, destination);
        return _mapper.Map<IEnumerable<RouteResponseDto>>(routes);
    }
    
    public async Task<IEnumerable<RouteResponseDto>> GetActiveRoutesAsync()
    {
        var routes = await _unitOfWork.Routes.GetActiveRoutesAsync();
        return _mapper.Map<IEnumerable<RouteResponseDto>>(routes);
    }
}
