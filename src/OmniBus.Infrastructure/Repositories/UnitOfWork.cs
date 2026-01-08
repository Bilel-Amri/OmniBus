using OmniBus.Domain.Interfaces;
using OmniBus.Infrastructure.Persistence;

namespace OmniBus.Infrastructure.Repositories;

/// <summary>
/// Unit of work implementation
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    
    private IUserRepository? _users;
    private IBusRepository? _buses;
    private IRouteRepository? _routes;
    private IScheduleRepository? _schedules;
    private ITicketRepository? _tickets;
    private IPaymentRepository? _payments;
    private ISeatLockRepository? _seatLocks;
    
    private bool _disposed;
    
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public IUserRepository Users => _users ??= new UserRepository(_context);
    public IBusRepository Buses => _buses ??= new BusRepository(_context);
    public IRouteRepository Routes => _routes ??= new RouteRepository(_context);
    public IScheduleRepository Schedules => _schedules ??= new ScheduleRepository(_context);
    public ITicketRepository Tickets => _tickets ??= new TicketRepository(_context);
    public IPaymentRepository Payments => _payments ??= new PaymentRepository(_context);
    public ISeatLockRepository SeatLocks => _seatLocks ??= new SeatLockRepository(_context);
    
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
    
    public async Task BeginTransactionAsync()
    {
        await _context.Database.BeginTransactionAsync();
    }
    
    public async Task CommitTransactionAsync()
    {
        if (_context.Database.CurrentTransaction != null)
        {
            await _context.Database.CommitTransactionAsync();
        }
    }
    
    public async Task RollbackTransactionAsync()
    {
        if (_context.Database.CurrentTransaction != null)
        {
            await _context.Database.RollbackTransactionAsync();
        }
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }
    }
}
