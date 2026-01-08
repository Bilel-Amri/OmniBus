using OmniBus.Application.DTOs;

namespace OmniBus.Application.Interfaces;

public interface IScheduleService
{
    Task<IEnumerable<ScheduleResponseDto>> GetAllSchedulesAsync();
    Task<ScheduleResponseDto?> GetScheduleByIdAsync(Guid id);
    Task<ScheduleResponseDto> CreateScheduleAsync(CreateScheduleDto request);
    Task<ScheduleResponseDto?> UpdateScheduleAsync(Guid id, UpdateScheduleDto request);
    Task<bool> DeleteScheduleAsync(Guid id);
    Task<IEnumerable<ScheduleResponseDto>> SearchSchedulesAsync(SearchScheduleDto request);
    Task<IEnumerable<ScheduleResponseDto>> GetSchedulesByDateAsync(DateTime date);
    Task<IEnumerable<ScheduleResponseDto>> GetSchedulesByDriverAsync(Guid driverId);
    Task<IEnumerable<ScheduleResponseDto>> GetSchedulesByRouteAsync(Guid routeId);
    Task<ScheduleSeatLayoutDto?> GetSeatLayoutAsync(Guid scheduleId);
    Task<IEnumerable<ScheduleResponseDto>> GetActiveSchedulesWithLocationAsync();
    Task<bool> UpdateScheduleLocationAsync(Guid id, LocationUpdateDto location);
}
