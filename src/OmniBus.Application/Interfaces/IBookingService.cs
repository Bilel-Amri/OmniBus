using System.Collections.Generic;
using System.Threading.Tasks;
using OmniBus.Application.DTOs;
using OmniBus.Domain.Entities;

namespace OmniBus.Application.Interfaces
{
    public interface IBookingService
    {
        Task<List<BookingDto>> GetAllBookingsAsync();
        Task<BookingDto?> GetBookingByIdAsync(int id);
        Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto bookingDto);
        Task<BookingDto?> UpdateBookingStatusAsync(int id, string status);
        Task<bool> CancelBookingAsync(int id);
        Task<List<BookingDto>> GetUserBookingsAsync(Guid userId);
        Task<List<BookingDto>> GetBookingsByTripAsync(int tripId);
    }
}