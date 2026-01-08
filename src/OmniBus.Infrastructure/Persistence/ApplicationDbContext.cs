using Microsoft.EntityFrameworkCore;
using OmniBus.Domain.Entities;
using OmniBus.Domain.Enums;

namespace OmniBus.Infrastructure.Persistence;

/// <summary>
/// Application database context
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    // DbSets
    public DbSet<User> Users => Set<User>();
    public DbSet<Bus> Buses => Set<Bus>();
    public DbSet<Route> Routes => Set<Route>();
    public DbSet<RouteStop> RouteStops => Set<RouteStop>();
    public DbSet<Schedule> Schedules => Set<Schedule>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<SeatLock> SeatLocks => Set<SeatLock>();
    public DbSet<Seat> Seats => Set<Seat>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Driver> Drivers => Set<Driver>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply all entity configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        
        // Seed data
        SeedData(modelBuilder);
    }
    
    private void SeedData(ModelBuilder modelBuilder)
    {
        // Use a fixed UTC datetime for seeding
        var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        // Seed Admin User
        var adminUser = new User
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            FirstName = "System",
            LastName = "Administrator",
            Email = "admin@omnibus.tn",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            Role = UserRole.Admin,
            EmailVerified = true,
            CreatedAt = seedDate,
            IsDeleted = false
        };
        modelBuilder.Entity<User>().HasData(adminUser);
        
        // Seed Sample Drivers
        var driver1 = new User
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            FirstName = "Ahmed",
            LastName = "Ben Ali",
            Email = "driver1@omnibus.tn",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Driver123!"),
            Role = UserRole.Driver,
            PhoneNumber = "+216 98 123 456",
            EmailVerified = true,
            CreatedAt = seedDate,
            IsDeleted = false
        };
        
        var driver2 = new User
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            FirstName = "Mohamed",
            LastName = "Trabelsi",
            Email = "driver2@omnibus.tn",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Driver123!"),
            Role = UserRole.Driver,
            PhoneNumber = "+216 97 654 321",
            EmailVerified = true,
            CreatedAt = seedDate,
            IsDeleted = false
        };
        modelBuilder.Entity<User>().HasData(driver1, driver2);
        
        // Seed Sample Buses with driver assignments
        var bus1 = new Bus
        {
            Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            PlateNumber = "TN 1234",
            RegistrationNumber = "REG-001",
            Brand = "Mercedes-Benz",
            Model = "Sprinter",
            YearManufactured = 2022,
            Capacity = 45,
            AvailableSeats = 45,
            Type = BusType.Intercity,
            Status = BusStatus.Active,
            SeatsPerRow = 4,
            HasAirConditioning = true,
            HasWifi = true,
            IsWheelchairAccessible = true,
            CurrentDriverId = driver1.Id,
            CreatedAt = seedDate,
            IsDeleted = false
        };
        
        var bus2 = new Bus
        {
            Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            PlateNumber = "TN 5678",
            RegistrationNumber = "REG-002",
            Brand = "Volvo",
            Model = "9900",
            YearManufactured = 2023,
            Capacity = 52,
            AvailableSeats = 52,
            Type = BusType.Express,
            Status = BusStatus.Active,
            SeatsPerRow = 4,
            HasAirConditioning = true,
            HasWifi = true,
            IsWheelchairAccessible = false,
            CurrentDriverId = driver2.Id,
            CreatedAt = seedDate,
            IsDeleted = false
        };
        
        var bus3 = new Bus
        {
            Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
            PlateNumber = "TN 9012",
            RegistrationNumber = "REG-003",
            Brand = "Iveco",
            Model = "Daily",
            YearManufactured = 2021,
            Capacity = 30,
            AvailableSeats = 30,
            Type = BusType.City,
            Status = BusStatus.Active,
            SeatsPerRow = 3,
            HasAirConditioning = true,
            HasWifi = false,
            IsWheelchairAccessible = true,
            CreatedAt = seedDate,
            IsDeleted = false
        };
        modelBuilder.Entity<Bus>().HasData(bus1, bus2, bus3);
        
        // Seed Routes
        var route1 = new Route
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111112"),
            Name = "Tunis - Sfax",
            Origin = "Tunis",
            OriginCode = "TUN",
            Destination = "Sfax",
            DestinationCode = "SFX",
            DistanceKm = 270,
            EstimatedDurationMinutes = 270,
            Description = "Main intercity route connecting Tunisia's capital to Sfax",
            IsActive = true,
            CreatedAt = seedDate,
            IsDeleted = false
        };
        
        var route2 = new Route
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222223"),
            Name = "Tunis - Sousse",
            Origin = "Tunis",
            OriginCode = "TUN",
            Destination = "Sousse",
            DestinationCode = "SUS",
            DistanceKm = 140,
            EstimatedDurationMinutes = 140,
            Description = "Express route to Sousse",
            IsActive = true,
            CreatedAt = seedDate,
            IsDeleted = false
        };
        
        var route3 = new Route
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333334"),
            Name = "Tunis - Bizerte",
            Origin = "Tunis",
            OriginCode = "TUN",
            Destination = "Bizerte",
            DestinationCode = "BIZ",
            DistanceKm = 70,
            EstimatedDurationMinutes = 70,
            Description = "Route to Bizerte",
            IsActive = true,
            CreatedAt = seedDate,
            IsDeleted = false
        };
        var route4 = new Route
        {
            Id = Guid.Parse("44444444-4444-4444-4444-444444444446"),
            Name = "National North-South",
            Origin = "Bizerte",
            OriginCode = "BIZ",
            Destination = "Tataouine",
            DestinationCode = "TAT",
            DistanceKm = 760,
            EstimatedDurationMinutes = 840,
            Description = "Major national spine covering key Tunisian cities",
            IsActive = true,
            CreatedAt = seedDate,
            IsDeleted = false
        };
        modelBuilder.Entity<Route>().HasData(route1, route2, route3, route4);
        
        // Seed Route Stops for Tunis-Sfax route
        modelBuilder.Entity<RouteStop>().HasData(
            new RouteStop
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444445"),
                Name = "Tunis Central Station",
                Order = 1,
                DistanceFromOrigin = 0,
                Latitude = 36.8065,
                Longitude = 10.1815,
                StopDurationMinutes = 5,
                RouteId = route1.Id,
                IsDeleted = false
            },
            new RouteStop
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555556"),
                Name = "Sidi Bou Said",
                Order = 2,
                DistanceFromOrigin = 15,
                Latitude = 36.8697,
                Longitude = 10.3423,
                StopDurationMinutes = 2,
                RouteId = route1.Id,
                IsDeleted = false
            },
            new RouteStop
            {
                Id = Guid.Parse("66666666-6666-6666-6666-666666666667"),
                Name = "Sfax Central Station",
                Order = 3,
                DistanceFromOrigin = 270,
                Latitude = 34.7405,
                Longitude = 10.7603,
                StopDurationMinutes = 0,
                RouteId = route1.Id,
                IsDeleted = false
            }
        );

        // Seed Route Stops for national spine (Bizerte to Tataouine)
        modelBuilder.Entity<RouteStop>().HasData(
            new RouteStop
            {
                Id = Guid.Parse("77777777-7777-7777-7777-777777777771"),
                Name = "Bizerte Station",
                Order = 1,
                DistanceFromOrigin = 0,
                Latitude = 37.2746,
                Longitude = 9.8739,
                StopDurationMinutes = 5,
                RouteId = route4.Id,
                IsDeleted = false
            },
            new RouteStop
            {
                Id = Guid.Parse("77777777-7777-7777-7777-777777777772"),
                Name = "Nabeul",
                Order = 2,
                DistanceFromOrigin = 95,
                Latitude = 36.4513,
                Longitude = 10.7350,
                StopDurationMinutes = 3,
                RouteId = route4.Id,
                IsDeleted = false
            },
            new RouteStop
            {
                Id = Guid.Parse("77777777-7777-7777-7777-777777777773"),
                Name = "Hammamet",
                Order = 3,
                DistanceFromOrigin = 115,
                Latitude = 36.4047,
                Longitude = 10.6143,
                StopDurationMinutes = 3,
                RouteId = route4.Id,
                IsDeleted = false
            },
            new RouteStop
            {
                Id = Guid.Parse("77777777-7777-7777-7777-777777777774"),
                Name = "Zaghouan",
                Order = 4,
                DistanceFromOrigin = 150,
                Latitude = 36.3993,
                Longitude = 10.1422,
                StopDurationMinutes = 2,
                RouteId = route4.Id,
                IsDeleted = false
            },
            new RouteStop
            {
                Id = Guid.Parse("77777777-7777-7777-7777-777777777775"),
                Name = "Kairouan",
                Order = 5,
                DistanceFromOrigin = 200,
                Latitude = 35.6769,
                Longitude = 10.1010,
                StopDurationMinutes = 5,
                RouteId = route4.Id,
                IsDeleted = false
            },
            new RouteStop
            {
                Id = Guid.Parse("77777777-7777-7777-7777-777777777776"),
                Name = "Sousse",
                Order = 6,
                DistanceFromOrigin = 235,
                Latitude = 35.8256,
                Longitude = 10.6411,
                StopDurationMinutes = 5,
                RouteId = route4.Id,
                IsDeleted = false
            },
            new RouteStop
            {
                Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                Name = "Monastir",
                Order = 7,
                DistanceFromOrigin = 255,
                Latitude = 35.7643,
                Longitude = 10.8113,
                StopDurationMinutes = 3,
                RouteId = route4.Id,
                IsDeleted = false
            },
            new RouteStop
            {
                Id = Guid.Parse("77777777-7777-7777-7777-777777777778"),
                Name = "Mahdia",
                Order = 8,
                DistanceFromOrigin = 295,
                Latitude = 35.5039,
                Longitude = 11.0457,
                StopDurationMinutes = 3,
                RouteId = route4.Id,
                IsDeleted = false
            },
            new RouteStop
            {
                Id = Guid.Parse("77777777-7777-7777-7777-777777777779"),
                Name = "Sfax",
                Order = 9,
                DistanceFromOrigin = 350,
                Latitude = 34.7405,
                Longitude = 10.7603,
                StopDurationMinutes = 5,
                RouteId = route4.Id,
                IsDeleted = false
            },
            new RouteStop
            {
                Id = Guid.Parse("77777777-7777-7777-7777-777777777780"),
                Name = "Gabes",
                Order = 10,
                DistanceFromOrigin = 440,
                Latitude = 33.8815,
                Longitude = 10.0982,
                StopDurationMinutes = 4,
                RouteId = route4.Id,
                IsDeleted = false
            },
            new RouteStop
            {
                Id = Guid.Parse("77777777-7777-7777-7777-777777777781"),
                Name = "Medenine",
                Order = 11,
                DistanceFromOrigin = 520,
                Latitude = 33.3549,
                Longitude = 10.5055,
                StopDurationMinutes = 4,
                RouteId = route4.Id,
                IsDeleted = false
            },
            new RouteStop
            {
                Id = Guid.Parse("77777777-7777-7777-7777-777777777782"),
                Name = "Tataouine",
                Order = 12,
                DistanceFromOrigin = 760,
                Latitude = 32.9297,
                Longitude = 10.4518,
                StopDurationMinutes = 0,
                RouteId = route4.Id,
                IsDeleted = false
            },
            new RouteStop
            {
                Id = Guid.Parse("77777777-7777-7777-7777-777777777783"),
                Name = "Kef",
                Order = 13,
                DistanceFromOrigin = 310,
                Latitude = 36.1769,
                Longitude = 8.7043,
                StopDurationMinutes = 3,
                RouteId = route4.Id,
                IsDeleted = false
            },
            new RouteStop
            {
                Id = Guid.Parse("77777777-7777-7777-7777-777777777784"),
                Name = "Beja",
                Order = 14,
                DistanceFromOrigin = 240,
                Latitude = 36.7333,
                Longitude = 9.1833,
                StopDurationMinutes = 3,
                RouteId = route4.Id,
                IsDeleted = false
            },
            new RouteStop
            {
                Id = Guid.Parse("77777777-7777-7777-7777-777777777785"),
                Name = "Jendouba",
                Order = 15,
                DistanceFromOrigin = 270,
                Latitude = 36.5011,
                Longitude = 8.7802,
                StopDurationMinutes = 3,
                RouteId = route4.Id,
                IsDeleted = false
            },
            new RouteStop
            {
                Id = Guid.Parse("77777777-7777-7777-7777-777777777786"),
                Name = "Siliana",
                Order = 16,
                DistanceFromOrigin = 210,
                Latitude = 36.0833,
                Longitude = 9.3667,
                StopDurationMinutes = 3,
                RouteId = route4.Id,
                IsDeleted = false
            },
            new RouteStop
            {
                Id = Guid.Parse("77777777-7777-7777-7777-777777777787"),
                Name = "Kasserine",
                Order = 17,
                DistanceFromOrigin = 360,
                Latitude = 35.1676,
                Longitude = 8.8365,
                StopDurationMinutes = 4,
                RouteId = route4.Id,
                IsDeleted = false
            },
            new RouteStop
            {
                Id = Guid.Parse("77777777-7777-7777-7777-777777777788"),
                Name = "Gafsa",
                Order = 18,
                DistanceFromOrigin = 420,
                Latitude = 34.4230,
                Longitude = 8.7841,
                StopDurationMinutes = 4,
                RouteId = route4.Id,
                IsDeleted = false
            },
            new RouteStop
            {
                Id = Guid.Parse("77777777-7777-7777-7777-777777777789"),
                Name = "Tozeur",
                Order = 19,
                DistanceFromOrigin = 520,
                Latitude = 33.9197,
                Longitude = 8.1335,
                StopDurationMinutes = 4,
                RouteId = route4.Id,
                IsDeleted = false
            }
        );
        
        // Seed Schedules - use future dates (2026)
        var scheduleTomorrow = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc);
        var schedule1 = new Schedule
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111113"),
            BusId = bus1.Id,
            RouteId = route1.Id,
            DriverId = driver1.Id,
            DepartureTime = scheduleTomorrow.AddHours(8),
            ArrivalTime = scheduleTomorrow.AddHours(13).AddMinutes(30),
            BasePrice = 25.00m,
            AvailableSeats = 40,
            Status = ScheduleStatus.Scheduled,
            OperatingDaysJson = "[0,1,2,3,4,5,6]",
            IsRecurring = true,
            CreatedAt = seedDate,
            IsDeleted = false
        };
        
        var schedule2 = new Schedule
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222224"),
            BusId = bus2.Id,
            RouteId = route2.Id,
            DriverId = driver2.Id,
            DepartureTime = scheduleTomorrow.AddHours(9),
            ArrivalTime = scheduleTomorrow.AddHours(11).AddMinutes(20),
            BasePrice = 15.00m,
            AvailableSeats = 48,
            Status = ScheduleStatus.Scheduled,
            OperatingDaysJson = "[0,1,2,3,4,5,6]",
            IsRecurring = true,
            CreatedAt = seedDate,
            IsDeleted = false
        };
        
        var schedule3 = new Schedule
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333335"),
            BusId = bus3.Id,
            RouteId = route3.Id,
            DriverId = driver1.Id,
            DepartureTime = scheduleTomorrow.AddHours(7).AddMinutes(30),
            ArrivalTime = scheduleTomorrow.AddHours(8).AddMinutes(40),
            BasePrice = 5.00m,
            AvailableSeats = 25,
            Status = ScheduleStatus.Scheduled,
            OperatingDaysJson = "[0,1,2,3,4,5,6]",
            IsRecurring = true,
            CreatedAt = seedDate,
            IsDeleted = false
        };
        modelBuilder.Entity<Schedule>().HasData(schedule1, schedule2, schedule3);
        
        // Load comprehensive seed data
        ComprehensiveSeedData.Seed(modelBuilder);
    }
}
