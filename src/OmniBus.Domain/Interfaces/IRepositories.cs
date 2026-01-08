using OmniBus.Domain.Entities;
using OmniBus.Domain.Enums;

namespace OmniBus.Domain.Interfaces;

/// <summary>
/// Repository interface for User entities
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task<IEnumerable<User>> GetDriversAsync();
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> ExistsByEmailAsync(string email);
}

/// <summary>
/// Repository interface for Bus entities
/// </summary>
public interface IBusRepository
{
    Task<Bus?> GetByIdAsync(Guid id);
    Task<Bus?> GetByPlateNumberAsync(string plateNumber);
    Task<IEnumerable<Bus>> GetAllAsync();
    Task<IEnumerable<Bus>> GetByStatusAsync(BusStatus status);
    Task<IEnumerable<Bus>> GetByTypeAsync(BusType type);
    Task<Bus> CreateAsync(Bus bus);
    Task<Bus> UpdateAsync(Bus bus);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task UpdateBusLocationAsync(Guid busId, double latitude, double longitude);
}

/// <summary>
/// Repository interface for Route entities
/// </summary>
public interface IRouteRepository
{
    Task<Route?> GetByIdAsync(Guid id);
    Task<IEnumerable<Route>> GetAllAsync();
    Task<IEnumerable<Route>> GetActiveRoutesAsync();
    Task<IEnumerable<Route>> SearchRoutesAsync(string origin, string destination);
    Task<Route> CreateAsync(Route route);
    Task<Route> UpdateAsync(Route route);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}

/// <summary>
/// Repository interface for Schedule entities
/// </summary>
public interface IScheduleRepository
{
    Task<Schedule?> GetByIdAsync(Guid id);
    Task<IEnumerable<Schedule>> GetAllAsync();
    Task<IEnumerable<Schedule>> GetByRouteAsync(Guid routeId);
    Task<IEnumerable<Schedule>> GetByBusAsync(Guid busId);
    Task<IEnumerable<Schedule>> GetByDriverAsync(Guid driverId);
    Task<IEnumerable<Schedule>> GetUpcomingSchedulesAsync(DateTime fromDate);
    Task<IEnumerable<Schedule>> GetSchedulesByDateAsync(DateTime date);
    Task<IEnumerable<Schedule>> SearchSchedulesAsync(string origin, string destination, DateTime date);
    Task<Schedule> CreateAsync(Schedule schedule);
    Task<Schedule> UpdateAsync(Schedule schedule);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task UpdateScheduleLocationAsync(Guid scheduleId, double latitude, double longitude);
    Task DecrementAvailableSeatsAsync(Guid scheduleId, int count = 1);
    Task IncrementAvailableSeatsAsync(Guid scheduleId, int count = 1);
}

/// <summary>
/// Repository interface for Ticket entities
/// </summary>
public interface ITicketRepository
{
    Task<Ticket?> GetByIdAsync(Guid id);
    Task<Ticket?> GetByBookingReferenceAsync(string reference);
    Task<IEnumerable<Ticket>> GetAllAsync();
    Task<IEnumerable<Ticket>> GetByUserAsync(Guid userId);
    Task<IEnumerable<Ticket>> GetByScheduleAsync(Guid scheduleId);
    Task<IEnumerable<Ticket>> GetUpcomingByUserAsync(Guid userId);
    Task<IEnumerable<Ticket>> GetCompletedByUserAsync(Guid userId);
    Task<Ticket> CreateAsync(Ticket ticket);
    Task<Ticket> UpdateAsync(Ticket ticket);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> IsSeatBookedAsync(Guid scheduleId, int seatNumber);
    Task<IEnumerable<int>> GetBookedSeatsAsync(Guid scheduleId);
    Task<string> GenerateBookingReferenceAsync();
}

/// <summary>
/// Repository interface for Payment entities
/// </summary>
public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(Guid id);
    Task<Payment?> GetByTransactionIdAsync(string transactionId);
    Task<IEnumerable<Payment>> GetAllAsync();
    Task<IEnumerable<Payment>> GetByUserAsync(Guid userId);
    Task<IEnumerable<Payment>> GetByTicketAsync(Guid ticketId);
    Task<Payment> CreateAsync(Payment payment);
    Task<Payment> UpdateAsync(Payment payment);
    Task<IEnumerable<Payment>> GetPaymentsByDateRangeAsync(DateTime from, DateTime to);
}

/// <summary>
/// Repository interface for SeatLock entities
/// </summary>
public interface ISeatLockRepository
{
    Task<SeatLock?> GetByIdAsync(Guid id);
    Task<SeatLock?> GetActiveLockAsync(Guid scheduleId, int seatNumber);
    Task<SeatLock?> GetUserActiveLockAsync(Guid scheduleId, Guid userId);
    Task<IEnumerable<SeatLock>> GetExpiredLocksAsync();
    Task<IEnumerable<SeatLock>> GetScheduleLocksAsync(Guid scheduleId);
    Task<SeatLock> CreateAsync(SeatLock seatLock);
    Task<SeatLock> UpdateAsync(SeatLock seatLock);
    Task<bool> DeleteAsync(Guid id);
    Task ReleaseExpiredLocksAsync();
    Task ReleaseUserLocksAsync(Guid userId, string sessionId);
}
