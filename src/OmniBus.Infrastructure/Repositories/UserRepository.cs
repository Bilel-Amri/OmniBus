using Microsoft.EntityFrameworkCore;
using OmniBus.Domain.Entities;
using OmniBus.Domain.Enums;
using OmniBus.Domain.Interfaces;
using OmniBus.Infrastructure.Persistence;

namespace OmniBus.Infrastructure.Repositories;

/// <summary>
/// User repository implementation
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    
    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
    }
    
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && !u.IsDeleted);
    }
    
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users
            .Where(u => !u.IsDeleted)
            .OrderBy(u => u.CreatedAt)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<User>> GetDriversAsync()
    {
        return await _context.Users
            .Where(u => u.Role == UserRole.Driver && !u.IsDeleted)
            .OrderBy(u => u.LastName)
            .ToListAsync();
    }
    
    public async Task<User> CreateAsync(User user)
    {
        user.CreatedAt = DateTime.UtcNow;
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
    
    public async Task<User> UpdateAsync(User user)
    {
        user.UpdatedAt = DateTime.UtcNow;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }
    
    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await GetByIdAsync(id);
        if (user == null) return false;
        
        user.IsDeleted = true;
        user.UpdatedAt = DateTime.UtcNow;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Users
            .AnyAsync(u => u.Id == id && !u.IsDeleted);
    }
    
    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Users
            .AnyAsync(u => u.Email.ToLower() == email.ToLower() && !u.IsDeleted);
    }
}

/// <summary>
/// Bus repository implementation
/// </summary>
public class BusRepository : IBusRepository
{
    private readonly ApplicationDbContext _context;
    
    public BusRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Bus?> GetByIdAsync(Guid id)
    {
        return await _context.Buses
            .Include(b => b.CurrentDriver)
            .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
    }
    
    public async Task<Bus?> GetByPlateNumberAsync(string plateNumber)
    {
        return await _context.Buses
            .FirstOrDefaultAsync(b => b.PlateNumber.ToUpper() == plateNumber.ToUpper() && !b.IsDeleted);
    }
    
    public async Task<IEnumerable<Bus>> GetAllAsync()
    {
        return await _context.Buses
            .Include(b => b.CurrentDriver)
            .Where(b => !b.IsDeleted)
            .OrderBy(b => b.CreatedAt)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Bus>> GetByStatusAsync(BusStatus status)
    {
        return await _context.Buses
            .Include(b => b.CurrentDriver)
            .Where(b => b.Status == status && !b.IsDeleted)
            .OrderBy(b => b.PlateNumber)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Bus>> GetByTypeAsync(BusType type)
    {
        return await _context.Buses
            .Include(b => b.CurrentDriver)
            .Where(b => b.Type == type && !b.IsDeleted)
            .OrderBy(b => b.PlateNumber)
            .ToListAsync();
    }
    
    public async Task<Bus> CreateAsync(Bus bus)
    {
        bus.CreatedAt = DateTime.UtcNow;
        bus.AvailableSeats = bus.Capacity;
        _context.Buses.Add(bus);
        await _context.SaveChangesAsync();
        return bus;
    }
    
    public async Task<Bus> UpdateAsync(Bus bus)
    {
        bus.UpdatedAt = DateTime.UtcNow;
        _context.Buses.Update(bus);
        await _context.SaveChangesAsync();
        return bus;
    }
    
    public async Task<bool> DeleteAsync(Guid id)
    {
        var bus = await GetByIdAsync(id);
        if (bus == null) return false;
        
        bus.IsDeleted = true;
        bus.UpdatedAt = DateTime.UtcNow;
        _context.Buses.Update(bus);
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Buses
            .AnyAsync(b => b.Id == id && !b.IsDeleted);
    }
    
    public async Task UpdateBusLocationAsync(Guid busId, double latitude, double longitude)
    {
        var bus = await GetByIdAsync(busId);
        if (bus != null)
        {
            bus.CurrentLatitude = latitude;
            bus.CurrentLongitude = longitude;
            bus.LastLocationUpdate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}

/// <summary>
/// Route repository implementation
/// </summary>
public class RouteRepository : IRouteRepository
{
    private readonly ApplicationDbContext _context;
    
    public RouteRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Route?> GetByIdAsync(Guid id)
    {
        return await _context.Routes
            .Include(r => r.Stops.OrderBy(s => s.Order))
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
    }
    
    public async Task<IEnumerable<Route>> GetAllAsync()
    {
        return await _context.Routes
            .Include(r => r.Stops.OrderBy(s => s.Order))
            .Where(r => !r.IsDeleted)
            .OrderBy(r => r.Name)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Route>> GetActiveRoutesAsync()
    {
        return await _context.Routes
            .Include(r => r.Stops.OrderBy(s => s.Order))
            .Where(r => r.IsActive && !r.IsDeleted)
            .OrderBy(r => r.Name)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Route>> SearchRoutesAsync(string origin, string destination)
    {
        return await _context.Routes
            .Include(r => r.Stops.OrderBy(s => s.Order))
            .Where(r => r.IsActive && !r.IsDeleted &&
                   (string.IsNullOrEmpty(origin) || r.Origin.ToLower().Contains(origin.ToLower())) &&
                   (string.IsNullOrEmpty(destination) || r.Destination.ToLower().Contains(destination.ToLower())))
            .OrderBy(r => r.Name)
            .ToListAsync();
    }
    
    public async Task<Route> CreateAsync(Route route)
    {
        route.CreatedAt = DateTime.UtcNow;
        if (route.Stops != null && route.Stops.Any())
        {
            foreach (var stop in route.Stops)
            {
                stop.RouteId = route.Id;
            }
        }
        _context.Routes.Add(route);
        await _context.SaveChangesAsync();
        return route;
    }
    
    public async Task<Route> UpdateAsync(Route route)
    {
        route.UpdatedAt = DateTime.UtcNow;
        _context.Routes.Update(route);
        await _context.SaveChangesAsync();
        return route;
    }
    
    public async Task<bool> DeleteAsync(Guid id)
    {
        var route = await GetByIdAsync(id);
        if (route == null) return false;
        
        route.IsDeleted = true;
        route.UpdatedAt = DateTime.UtcNow;
        _context.Routes.Update(route);
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Routes
            .AnyAsync(r => r.Id == id && !r.IsDeleted);
    }
}
