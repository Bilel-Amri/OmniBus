using OmniBus.Application.DTOs;
using OmniBus.Domain.Enums;

namespace OmniBus.Application.Interfaces;

public interface IBusService
{
    Task<IEnumerable<BusResponseDto>> GetAllBusesAsync();
    Task<BusResponseDto?> GetBusByIdAsync(Guid id);
    Task<BusResponseDto> CreateBusAsync(CreateBusDto request);
    Task<BusResponseDto?> UpdateBusAsync(Guid id, UpdateBusDto request);
    Task<bool> DeleteBusAsync(Guid id);
    Task<IEnumerable<BusResponseDto>> GetBusesByStatusAsync(BusStatus status);
    Task<IEnumerable<BusResponseDto>> GetBusesByTypeAsync(BusType type);
    Task<bool> UpdateBusLocationAsync(Guid busId, LocationUpdateDto location);
    Task<IEnumerable<BusResponseDto>> GetActiveBusesWithLocationAsync();
}
