using System.Collections.Generic;
using System.Threading.Tasks;
using OmniBus.Application.DTOs;
using OmniBus.Domain.Entities;

namespace OmniBus.Application.Interfaces
{
    public interface ISeatService
    {
        Task<List<SeatDto>> GetSeatsByBusIdAsync(int busId);
        Task<SeatDto?> GetSeatByIdAsync(int id);
        Task<SeatDto> CreateSeatAsync(SeatDto seatDto);
        Task<SeatDto?> UpdateSeatAsync(int id, SeatDto seatDto);
        Task<bool> DeleteSeatAsync(int id);
        Task<List<SeatLayoutDto>> GetSeatLayoutByBusIdAsync(int busId);
        Task<bool> LockSeatAsync(int seatId, Guid bookingId);
        Task<bool> UnlockSeatAsync(int seatId);
        Task<List<SeatDto>> GetAvailableSeatsAsync(int busId, int scheduleId);
    }
}