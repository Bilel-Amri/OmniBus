namespace OmniBus.Domain.Interfaces;

/// <summary>
/// Unit of work interface for transactional operations
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IBusRepository Buses { get; }
    IRouteRepository Routes { get; }
    IScheduleRepository Schedules { get; }
    ITicketRepository Tickets { get; }
    IPaymentRepository Payments { get; }
    ISeatLockRepository SeatLocks { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
