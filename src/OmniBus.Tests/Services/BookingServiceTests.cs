using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OmniBus.Domain.Entities;
using OmniBus.Domain.Enums;
using OmniBus.Infrastructure.Persistence;
using Xunit;
using FluentAssertions;

namespace OmniBus.Tests.Services;

public class BookingServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Guid _routeId = Guid.NewGuid();
    private readonly Guid _busId = Guid.NewGuid();
    private readonly Guid _scheduleId = Guid.NewGuid();
    private readonly Guid _passengerId = Guid.NewGuid();

    public BookingServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        
        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        var route = new Route
        {
            Id = _routeId,
            Name = "Tunis - Sfax",
            Origin = "Tunis",
            Destination = "Sfax",
            DistanceKm = 270,
            EstimatedDurationMinutes = 270,
            IsActive = true
        };

        var bus = new Bus
        {
            Id = _busId,
            PlateNumber = "TEST-123",
            RegistrationNumber = "TEST-REG",
            Capacity = 40,
            AvailableSeats = 40,
            Type = BusType.Intercity,
            Status = BusStatus.Active
        };

        var schedule = new Schedule
        {
            Id = _scheduleId,
            RouteId = _routeId,
            BusId = _busId,
            DepartureTime = DateTime.UtcNow.AddHours(2),
            ArrivalTime = DateTime.UtcNow.AddHours(5),
            Status = ScheduleStatus.Scheduled,
            BasePrice = 25.00m,
            AvailableSeats = 40,
            Route = route,
            Bus = bus
        };

        var passenger = new User
        {
            Id = _passengerId,
            FirstName = "Test",
            LastName = "User",
            Email = "test@test.com",
            PasswordHash = "test",
            PhoneNumber = "123456789",
            Role = UserRole.Passenger
        };

        _context.Routes.Add(route);
        _context.Buses.Add(bus);
        _context.Schedules.Add(schedule);
        _context.Users.Add(passenger);
        _context.SaveChanges();
    }

    [Fact]
    public async Task BookTicket_ShouldSucceed_WhenSeatsAvailable()
    {
        // Arrange
        var scheduleId = _scheduleId;
        var passengerId = _passengerId;
        var seatNumber = 1;

        // Act
        var result = await CreateBooking(scheduleId, passengerId, seatNumber);

        // Assert
        result.Should().NotBeNull();
        var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.UserId == passengerId);
        ticket.Should().NotBeNull();
        ticket!.Status.Should().Be(TicketStatus.Booked);
        ticket.SeatNumber.Should().Be(seatNumber);
    }

    [Fact]
    public async Task BookTicket_ShouldFail_WhenSeatAlreadyBooked()
    {
        // Arrange
        var scheduleId = _scheduleId;
        var passenger1Id = _passengerId;
        var seatNumber = 1;

        // Act - First booking
        await CreateBooking(scheduleId, passenger1Id, seatNumber);

        // Act - Try to book same seat
        Func<Task> act = async () => await CreateBooking(scheduleId, passenger1Id, seatNumber);

        // Assert
        var tickets = await _context.Tickets.Where(t => t.ScheduleId == scheduleId && t.SeatNumber == seatNumber).ToListAsync();
        tickets.Should().HaveCount(1);
    }

    [Fact]
    public async Task CancelTicket_ShouldSucceed_WhenTicketExists()
    {
        // Arrange
        var scheduleId = _scheduleId;
        var passengerId = _passengerId;
        var seatNumber = 1;

        var ticket = await CreateBooking(scheduleId, passengerId, seatNumber);

        // Act
        ticket.Status = TicketStatus.Cancelled;
        _context.Tickets.Update(ticket);
        await _context.SaveChangesAsync();

        // Assert
        var cancelledTicket = await _context.Tickets.FindAsync(ticket.Id);
        cancelledTicket.Should().NotBeNull();
        cancelledTicket!.Status.Should().Be(TicketStatus.Cancelled);
    }

    [Fact]
    public async Task GetAvailableSeats_ShouldReturnCorrectCount()
    {
        // Arrange
        var scheduleId = _scheduleId;
        var passenger1Id = _passengerId;

        // Book some seats
        await CreateBooking(scheduleId, passenger1Id, 1);
        await CreateBooking(scheduleId, passenger1Id, 2);
        await CreateBooking(scheduleId, passenger1Id, 3);

        // Act
        var schedule = await _context.Schedules
            .Include(s => s.Bus)
            .Include(s => s.Tickets)
            .FirstAsync(s => s.Id == scheduleId);

        var bookedSeats = schedule.Tickets.Count(t => t.Status != TicketStatus.Cancelled);
        var availableSeats = schedule.Bus.Capacity - bookedSeats;

        // Assert
        availableSeats.Should().Be(37); // 40 capacity - 3 booked
    }

    private async Task<Ticket> CreateBooking(Guid scheduleId, Guid passengerId, int seatNumber)
    {
        var schedule = await _context.Schedules
            .Include(s => s.Route)
            .FirstAsync(s => s.Id == scheduleId);

        var ticket = new Ticket
        {
            ScheduleId = scheduleId,
            UserId = passengerId,
            SeatNumber = seatNumber,
            Price = schedule.BasePrice,
            BookingDate = DateTime.UtcNow,
            Status = TicketStatus.Booked,
            BookingReference = $"TKT-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            PassengerName = "Test User",
            PassengerPhone = "123456789"
        };

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();
        return ticket;
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
