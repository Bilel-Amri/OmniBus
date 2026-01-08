using OmniBus.Application.DTOs;

namespace OmniBus.Application.Interfaces;

public interface IRouteService
{
    Task<IEnumerable<RouteResponseDto>> GetAllRoutesAsync();
    Task<RouteResponseDto?> GetRouteByIdAsync(Guid id);
    Task<RouteResponseDto> CreateRouteAsync(CreateRouteDto request);
    Task<RouteResponseDto?> UpdateRouteAsync(Guid id, UpdateRouteDto request);
    Task<bool> DeleteRouteAsync(Guid id);
    Task<IEnumerable<RouteResponseDto>> GetActiveRoutesAsync();
    Task<IEnumerable<RouteResponseDto>> SearchRoutesAsync(string origin, string destination);
}
