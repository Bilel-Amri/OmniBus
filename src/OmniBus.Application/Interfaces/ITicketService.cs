using OmniBus.Application.DTOs;

namespace OmniBus.Application.Interfaces;

public interface ITicketService
{
    Task<IEnumerable<TicketResponseDto>> GetAllTicketsAsync();
    Task<TicketResponseDto?> GetTicketByIdAsync(Guid id);
    Task<TicketResponseDto?> GetTicketByReferenceAsync(string reference);
    Task<IEnumerable<MyTicketDto>> GetTicketsByUserAsync(Guid userId);
    Task<IEnumerable<MyTicketDto>> GetUpcomingTicketsByUserAsync(Guid userId);
    Task<IEnumerable<MyTicketDto>> GetCompletedTicketsByUserAsync(Guid userId);
    Task<SeatLockResponseDto?> LockSeatAsync(Guid userId, SeatLockRequestDto request, string sessionId);
    Task<bool> ReleaseSeatLockAsync(Guid lockId);
    Task<bool> ReleaseUserLocksAsync(Guid userId, string sessionId);
    Task<TicketResponseDto> BookTicketAsync(Guid userId, CreateTicketDto request);
    Task<TicketResponseDto?> CancelTicketAsync(Guid ticketId, CancelTicketDto request);
    Task<bool> ConfirmBoardingAsync(Guid ticketId, Guid driverId, string? qrCode);
    Task<IEnumerable<int>> GetBookedSeatsAsync(Guid scheduleId);
    Task<ScheduleSeatLayoutDto?> GetSeatLayoutWithAvailabilityAsync(Guid scheduleId, Guid? userId = null);
    Task<TicketStatsDto> GetTicketStatsAsync();
}
