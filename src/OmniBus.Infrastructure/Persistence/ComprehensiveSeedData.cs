using Microsoft.EntityFrameworkCore;
using OmniBus.Domain.Entities;
using OmniBus.Domain.Enums;

namespace OmniBus.Infrastructure.Persistence;

/// <summary>
/// Comprehensive seed data for complete demo/testing environment
/// </summary>
public static class ComprehensiveSeedData
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var now = DateTime.UtcNow;
        var today = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);

        // ====== ADDITIONAL DRIVERS ======
        var drivers = new[]
        {
            new User { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), FirstName = "Mohamed", LastName = "Trabelsi", Email = "driver4@omnibus.tn", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Driver123!"), Role = UserRole.Driver, PhoneNumber = "+216 98 100004", EmailVerified = true, CreatedAt = seedDate, IsDeleted = false },
            new User { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), FirstName = "Ahmed", LastName = "Bouazizi", Email = "driver5@omnibus.tn", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Driver123!"), Role = UserRole.Driver, PhoneNumber = "+216 98 100005", EmailVerified = true, CreatedAt = seedDate, IsDeleted = false },
            new User { Id = Guid.Parse("66666666-6666-6666-6666-666666666666"), FirstName = "Sami", LastName = "Kassem", Email = "driver6@omnibus.tn", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Driver123!"), Role = UserRole.Driver, PhoneNumber = "+216 98 100006", EmailVerified = true, CreatedAt = seedDate, IsDeleted = false },
            new User { Id = Guid.Parse("77777777-7777-7777-7777-777777777777"), FirstName = "Khalil", LastName = "Gharbi", Email = "driver7@omnibus.tn", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Driver123!"), Role = UserRole.Driver, PhoneNumber = "+216 98 100007", EmailVerified = true, CreatedAt = seedDate, IsDeleted = false },
            new User { Id = Guid.Parse("88888888-8888-8888-8888-888888888888"), FirstName = "Nabil", LastName = "Jendoubi", Email = "driver8@omnibus.tn", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Driver123!"), Role = UserRole.Driver, PhoneNumber = "+216 98 100008", EmailVerified = true, CreatedAt = seedDate, IsDeleted = false },
            new User { Id = Guid.Parse("99999999-9999-9999-9999-999999999999"), FirstName = "Youssef", LastName = "Mansour", Email = "driver9@omnibus.tn", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Driver123!"), Role = UserRole.Driver, PhoneNumber = "+216 98 100009", EmailVerified = true, CreatedAt = seedDate, IsDeleted = false },
            new User { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), FirstName = "Tarek", LastName = "Oueslati", Email = "driver10@omnibus.tn", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Driver123!"), Role = UserRole.Driver, PhoneNumber = "+216 98 100010", EmailVerified = true, CreatedAt = seedDate, IsDeleted = false }
        };
        modelBuilder.Entity<User>().HasData(drivers);

        // ====== SAMPLE PASSENGERS ======
        var passenger1 = new User
        {
            Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
            FirstName = "Fatma",
            LastName = "Ben Salem",
            Email = "fatma@example.tn",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pass123!"),
            Role = UserRole.Passenger,
            PhoneNumber = "+216 92 555 111",
            EmailVerified = true,
            CreatedAt = seedDate,
            IsDeleted = false
        };
        modelBuilder.Entity<User>().HasData(passenger1);

        // ====== ADDITIONAL BUSES ======
        var additionalBuses = new List<Bus>
        {
            new Bus
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                PlateNumber = "TN 3456",
                RegistrationNumber = "REG-004",
                Brand = "Scania",
                Model = "Interlink",
                YearManufactured = 2021,
                Capacity = 48,
                AvailableSeats = 48,
                Type = BusType.Intercity,
                Status = BusStatus.Active,
                SeatsPerRow = 4,
                HasAirConditioning = true,
                HasWifi = true,
                IsWheelchairAccessible = false,
                CurrentDriverId = drivers[0].Id,
                CurrentLatitude = 36.8065,
                CurrentLongitude = 10.1815,
                LastLocationUpdate = now.AddMinutes(-10),
                CreatedAt = seedDate,
                IsDeleted = false
            },
            new Bus
            {
                Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                PlateNumber = "TN 7890",
                RegistrationNumber = "REG-005",
                Brand = "Man",
                Model = "Lion's Coach",
                YearManufactured = 2022,
                Capacity = 55,
                AvailableSeats = 55,
                Type = BusType.Express,
                Status = BusStatus.Active,
                SeatsPerRow = 4,
                HasAirConditioning = true,
                HasWifi = true,
                IsWheelchairAccessible = true,
                CurrentDriverId = drivers[1].Id,
                CurrentLatitude = 35.8256,
                CurrentLongitude = 10.6411,
                LastLocationUpdate = now.AddMinutes(-5),
                CreatedAt = seedDate,
                IsDeleted = false
            },
            new Bus
            {
                Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                PlateNumber = "TN 1111",
                RegistrationNumber = "REG-006",
                Brand = "Mercedes",
                Model = "Citaro",
                YearManufactured = 2023,
                Capacity = 35,
                AvailableSeats = 35,
                Type = BusType.City,
                Status = BusStatus.Active,
                SeatsPerRow = 3,
                HasAirConditioning = true,
                HasWifi = false,
                IsWheelchairAccessible = true,
                CurrentDriverId = drivers[2].Id,
                CurrentLatitude = 34.7405,
                CurrentLongitude = 10.7603,
                LastLocationUpdate = now.AddMinutes(-15),
                CreatedAt = seedDate,
                IsDeleted = false
            }
        };
        modelBuilder.Entity<Bus>().HasData(additionalBuses);

        // ====== SNTRI INTERCITY ROUTES (Real Tunisia Routes) ======
        var additionalRoutes = new List<Route>
        {
            // North-South Main Corridor
            new Route { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Name = "Tunis - Sfax", Origin = "Tunis", OriginCode = "TUN", Destination = "Sfax", DestinationCode = "SFX", DistanceKm = 270, EstimatedDurationMinutes = 210, Description = "SNTRI main corridor route", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("66666666-6666-6666-6666-666666666666"), Name = "Sfax - Tunis", Origin = "Sfax", OriginCode = "SFX", Destination = "Tunis", DestinationCode = "TUN", DistanceKm = 270, EstimatedDurationMinutes = 210, Description = "SNTRI return route", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("77777777-7777-7777-7777-777777777777"), Name = "Tunis - Sousse", Origin = "Tunis", OriginCode = "TUN", Destination = "Sousse", DestinationCode = "SUS", DistanceKm = 140, EstimatedDurationMinutes = 120, Description = "SNTRI coastal route", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("88888888-8888-8888-8888-888888888888"), Name = "Sousse - Tunis", Origin = "Sousse", OriginCode = "SUS", Destination = "Tunis", DestinationCode = "TUN", DistanceKm = 140, EstimatedDurationMinutes = 120, Description = "SNTRI return coastal route", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("99999999-9999-9999-9999-999999999999"), Name = "Sfax - Gabes", Origin = "Sfax", OriginCode = "SFX", Destination = "Gabes", DestinationCode = "GAB", DistanceKm = 150, EstimatedDurationMinutes = 150, Description = "SNTRI southern route", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"), Name = "Gabes - Sfax", Origin = "Gabes", OriginCode = "GAB", Destination = "Sfax", DestinationCode = "SFX", DistanceKm = 150, EstimatedDurationMinutes = 150, Description = "SNTRI return southern route", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            
            // Northern Routes
            new Route { Id = Guid.Parse("bbbbbbbb-cccc-dddd-eeee-ffffffffffff"), Name = "Tunis - Bizerte", Origin = "Tunis", OriginCode = "TUN", Destination = "Bizerte", DestinationCode = "BIZ", DistanceKm = 70, EstimatedDurationMinutes = 60, Description = "SNTRI northern route", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("cccccccc-dddd-eeee-ffff-aaaaaaaaaaaa"), Name = "Bizerte - Tunis", Origin = "Bizerte", OriginCode = "BIZ", Destination = "Tunis", DestinationCode = "TUN", DistanceKm = 70, EstimatedDurationMinutes = 60, Description = "SNTRI return northern route", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            
            // Cap Bon Peninsula (Tourist Routes)
            new Route { Id = Guid.Parse("dddddddd-eeee-ffff-aaaa-bbbbbbbbbbbb"), Name = "Tunis - Nabeul", Origin = "Tunis", OriginCode = "TUN", Destination = "Nabeul", DestinationCode = "NAB", DistanceKm = 65, EstimatedDurationMinutes = 60, Description = "SNTRI Cap Bon route", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("eeeeeeee-ffff-aaaa-bbbb-cccccccccccc"), Name = "Nabeul - Tunis", Origin = "Nabeul", OriginCode = "NAB", Destination = "Tunis", DestinationCode = "TUN", DistanceKm = 65, EstimatedDurationMinutes = 60, Description = "SNTRI return Cap Bon", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("ffffffff-aaaa-bbbb-cccc-dddddddddddd"), Name = "Tunis - Hammamet", Origin = "Tunis", OriginCode = "TUN", Destination = "Hammamet", DestinationCode = "HAM", DistanceKm = 70, EstimatedDurationMinutes = 70, Description = "SNTRI tourist route", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("aaaaaaaa-1111-2222-3333-444444444444"), Name = "Hammamet - Tunis", Origin = "Hammamet", OriginCode = "HAM", Destination = "Tunis", DestinationCode = "TUN", DistanceKm = 70, EstimatedDurationMinutes = 70, Description = "SNTRI return tourist route", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("bbbbbbbb-2222-3333-4444-555555555555"), Name = "Nabeul - Hammamet", Origin = "Nabeul", OriginCode = "NAB", Destination = "Hammamet", DestinationCode = "HAM", DistanceKm = 15, EstimatedDurationMinutes = 20, Description = "SNTRI coastal connection", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            
            // Sahel Region
            new Route { Id = Guid.Parse("cccccccc-3333-4444-5555-666666666666"), Name = "Sousse - Monastir", Origin = "Sousse", OriginCode = "SUS", Destination = "Monastir", DestinationCode = "MNS", DistanceKm = 25, EstimatedDurationMinutes = 25, Description = "SNTRI Sahel route", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("dddddddd-4444-5555-6666-777777777777"), Name = "Monastir - Sousse", Origin = "Monastir", OriginCode = "MNS", Destination = "Sousse", DestinationCode = "SUS", DistanceKm = 25, EstimatedDurationMinutes = 25, Description = "SNTRI return Sahel route", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("eeeeeeee-5555-6666-7777-888888888888"), Name = "Sousse - Sfax", Origin = "Sousse", OriginCode = "SUS", Destination = "Sfax", DestinationCode = "SFX", DistanceKm = 120, EstimatedDurationMinutes = 100, Description = "SNTRI coastal southern route", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("ffffffff-6666-7777-8888-999999999999"), Name = "Sfax - Sousse", Origin = "Sfax", OriginCode = "SFX", Destination = "Sousse", DestinationCode = "SUS", DistanceKm = 120, EstimatedDurationMinutes = 100, Description = "SNTRI return coastal route", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            
            // Central Routes
            new Route { Id = Guid.Parse("aaaaaaaa-7777-8888-9999-aaaaaaaaaaaa"), Name = "Tunis - Kairouan", Origin = "Tunis", OriginCode = "TUN", Destination = "Kairouan", DestinationCode = "KAI", DistanceKm = 160, EstimatedDurationMinutes = 135, Description = "SNTRI central route to holy city", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("bbbbbbbb-8888-9999-aaaa-bbbbbbbbbbbb"), Name = "Kairouan - Tunis", Origin = "Kairouan", OriginCode = "KAI", Destination = "Tunis", DestinationCode = "TUN", DistanceKm = 160, EstimatedDurationMinutes = 135, Description = "SNTRI return central route", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("cccccccc-9999-aaaa-bbbb-cccccccccccc"), Name = "Kairouan - Sousse", Origin = "Kairouan", OriginCode = "KAI", Destination = "Sousse", DestinationCode = "SUS", DistanceKm = 60, EstimatedDurationMinutes = 60, Description = "SNTRI central to coast", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            
            // Southern Deep Routes
            new Route { Id = Guid.Parse("dddddddd-aaaa-bbbb-cccc-dddddddddddd"), Name = "Gabes - Medenine", Origin = "Gabes", OriginCode = "GAB", Destination = "Medenine", DestinationCode = "MED", DistanceKm = 80, EstimatedDurationMinutes = 75, Description = "SNTRI far south route", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("eeeeeeee-bbbb-cccc-dddd-eeeeeeeeeeee"), Name = "Medenine - Gabes", Origin = "Medenine", OriginCode = "MED", Destination = "Gabes", DestinationCode = "GAB", DistanceKm = 80, EstimatedDurationMinutes = 75, Description = "SNTRI return far south", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("ffffffff-cccc-dddd-eeee-ffffffffffff"), Name = "Gabes - Tataouine", Origin = "Gabes", OriginCode = "GAB", Destination = "Tataouine", DestinationCode = "TAT", DistanceKm = 140, EstimatedDurationMinutes = 150, Description = "SNTRI desert route", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("aaaaaaaa-dddd-eeee-ffff-aaaaaaa11111"), Name = "Tataouine - Gabes", Origin = "Tataouine", OriginCode = "TAT", Destination = "Gabes", DestinationCode = "GAB", DistanceKm = 140, EstimatedDurationMinutes = 150, Description = "SNTRI return desert route", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            
            // Long Distance Express
            new Route { Id = Guid.Parse("bbbbbbbb-eeee-ffff-aaaa-bbbbbbbb2222"), Name = "Tunis - Gabes", Origin = "Tunis", OriginCode = "TUN", Destination = "Gabes", DestinationCode = "GAB", DistanceKm = 400, EstimatedDurationMinutes = 360, Description = "SNTRI express long distance", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("cccccccc-ffff-aaaa-bbbb-cccccccc3333"), Name = "Gabes - Tunis", Origin = "Gabes", OriginCode = "GAB", Destination = "Tunis", DestinationCode = "TUN", DistanceKm = 400, EstimatedDurationMinutes = 360, Description = "SNTRI return express", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            
            // ====== ADDITIONAL MAJOR SNTRI ROUTES FROM CSV DATA ======
            
            // Western Routes to Gafsa Mining Region
            new Route { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Tunis - Gafsa", Origin = "Tunis", OriginCode = "TUN", Destination = "Gafsa", DestinationCode = "GAF", DistanceKm = 340, EstimatedDurationMinutes = 340, Description = "SNTRI Line 100 via Kairouan", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("22222222-1111-1111-1111-111111111111"), Name = "Gafsa - Tunis", Origin = "Gafsa", OriginCode = "GAF", Destination = "Tunis", DestinationCode = "TUN", DistanceKm = 340, EstimatedDurationMinutes = 340, Description = "SNTRI return Line 100", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("33333333-1111-1111-1111-111111111111"), Name = "Tunis - Metlaoui", Origin = "Tunis", OriginCode = "TUN", Destination = "Metlaoui", DestinationCode = "MTL", DistanceKm = 370, EstimatedDurationMinutes = 390, Description = "SNTRI Line 401 night service", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("44444444-1111-1111-1111-111111111111"), Name = "Metlaoui - Tunis", Origin = "Metlaoui", OriginCode = "MTL", Destination = "Tunis", DestinationCode = "TUN", DistanceKm = 370, EstimatedDurationMinutes = 390, Description = "SNTRI return Line 401", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            
            // Desert Oasis Routes
            new Route { Id = Guid.Parse("55555555-1111-1111-1111-111111111111"), Name = "Tunis - Tozeur", Origin = "Tunis", OriginCode = "TUN", Destination = "Tozeur", DestinationCode = "TOZ", DistanceKm = 420, EstimatedDurationMinutes = 420, Description = "SNTRI Line 142 via Gafsa", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("66666666-1111-1111-1111-111111111111"), Name = "Tozeur - Tunis", Origin = "Tozeur", OriginCode = "TOZ", Destination = "Tunis", DestinationCode = "TUN", DistanceKm = 420, EstimatedDurationMinutes = 420, Description = "SNTRI return Line 142", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("77777777-1111-1111-1111-111111111111"), Name = "Tunis - Nefta", Origin = "Tunis", OriginCode = "TUN", Destination = "Nefta", DestinationCode = "NEF", DistanceKm = 450, EstimatedDurationMinutes = 450, Description = "SNTRI Line 102/501 via Tozeur", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("88888888-1111-1111-1111-111111111111"), Name = "Nefta - Tunis", Origin = "Nefta", OriginCode = "NEF", Destination = "Tunis", DestinationCode = "TUN", DistanceKm = 450, EstimatedDurationMinutes = 450, Description = "SNTRI return Line 102/501", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("99999999-1111-1111-1111-111111111111"), Name = "Tunis - Douz", Origin = "Tunis", OriginCode = "TUN", Destination = "Douz", DestinationCode = "DOZ", DistanceKm = 480, EstimatedDurationMinutes = 540, Description = "SNTRI Line 502 Sahara gateway", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("aaaaaaaa-1111-1111-1111-111111111111"), Name = "Douz - Tunis", Origin = "Douz", OriginCode = "DOZ", Destination = "Tunis", DestinationCode = "TUN", DistanceKm = 480, EstimatedDurationMinutes = 540, Description = "SNTRI return Line 502", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            
            // Northwestern Mountain Routes
            new Route { Id = Guid.Parse("bbbbbbbb-1111-1111-1111-111111111111"), Name = "Tunis - Ain Draham", Origin = "Tunis", OriginCode = "TUN", Destination = "Ain Draham", DestinationCode = "AID", DistanceKm = 200, EstimatedDurationMinutes = 180, Description = "SNTRI Line 121 mountain resort", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("cccccccc-1111-1111-1111-111111111111"), Name = "Ain Draham - Tunis", Origin = "Ain Draham", OriginCode = "AID", Destination = "Tunis", DestinationCode = "TUN", DistanceKm = 200, EstimatedDurationMinutes = 180, Description = "SNTRI return Line 121", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("dddddddd-1111-1111-1111-111111111111"), Name = "Tunis - Tabarka", Origin = "Tunis", OriginCode = "TUN", Destination = "Tabarka", DestinationCode = "TAB", DistanceKm = 175, EstimatedDurationMinutes = 210, Description = "SNTRI Line 124 beach resort", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("eeeeeeee-1111-1111-1111-111111111111"), Name = "Tabarka - Tunis", Origin = "Tabarka", OriginCode = "TAB", Destination = "Tunis", DestinationCode = "TUN", DistanceKm = 175, EstimatedDurationMinutes = 210, Description = "SNTRI return Line 124", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("ffffffff-1111-1111-1111-111111111111"), Name = "Tunis - Jendouba", Origin = "Tunis", OriginCode = "TUN", Destination = "Jendouba", DestinationCode = "JEN", DistanceKm = 155, EstimatedDurationMinutes = 180, Description = "SNTRI Line 170 northwest", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("11111111-2222-1111-1111-111111111111"), Name = "Jendouba - Tunis", Origin = "Jendouba", OriginCode = "JEN", Destination = "Tunis", DestinationCode = "TUN", DistanceKm = 155, EstimatedDurationMinutes = 180, Description = "SNTRI return Line 170", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("22222222-2222-1111-1111-111111111111"), Name = "Tunis - Le Kef", Origin = "Tunis", OriginCode = "TUN", Destination = "Le Kef", DestinationCode = "KEF", DistanceKm = 175, EstimatedDurationMinutes = 180, Description = "SNTRI Line 109 historic city", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("33333333-2222-1111-1111-111111111111"), Name = "Le Kef - Tunis", Origin = "Le Kef", OriginCode = "KEF", Destination = "Tunis", DestinationCode = "TUN", DistanceKm = 175, EstimatedDurationMinutes = 180, Description = "SNTRI return Line 109", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            
            // Central Tunisia Routes
            new Route { Id = Guid.Parse("44444444-2222-1111-1111-111111111111"), Name = "Tunis - Sidi Bouzid", Origin = "Tunis", OriginCode = "TUN", Destination = "Sidi Bouzid", DestinationCode = "SBZ", DistanceKm = 250, EstimatedDurationMinutes = 240, Description = "SNTRI Line 105 central Tunisia", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("55555555-2222-1111-1111-111111111111"), Name = "Sidi Bouzid - Tunis", Origin = "Sidi Bouzid", OriginCode = "SBZ", Destination = "Tunis", DestinationCode = "TUN", DistanceKm = 250, EstimatedDurationMinutes = 240, Description = "SNTRI return Line 105", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("66666666-2222-1111-1111-111111111111"), Name = "Tunis - Kasserine", Origin = "Tunis", OriginCode = "TUN", Destination = "Kasserine", DestinationCode = "KAS", DistanceKm = 290, EstimatedDurationMinutes = 300, Description = "SNTRI Line 107 via Sbeitla", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("77777777-2222-1111-1111-111111111111"), Name = "Kasserine - Tunis", Origin = "Kasserine", OriginCode = "KAS", Destination = "Tunis", DestinationCode = "TUN", DistanceKm = 290, EstimatedDurationMinutes = 300, Description = "SNTRI return Line 107", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("88888888-2222-1111-1111-111111111111"), Name = "Tunis - Siliana", Origin = "Tunis", OriginCode = "TUN", Destination = "Siliana", DestinationCode = "SIL", DistanceKm = 135, EstimatedDurationMinutes = 165, Description = "SNTRI Line 300 via Gaafour", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("99999999-2222-1111-1111-111111111111"), Name = "Siliana - Tunis", Origin = "Siliana", OriginCode = "SIL", Destination = "Tunis", DestinationCode = "TUN", DistanceKm = 135, EstimatedDurationMinutes = 165, Description = "SNTRI return Line 300", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            
            // Deep South Routes
            new Route { Id = Guid.Parse("aaaaaaaa-2222-1111-1111-111111111111"), Name = "Tunis - Tataouine", Origin = "Tunis", OriginCode = "TUN", Destination = "Tataouine", DestinationCode = "TAT", DistanceKm = 530, EstimatedDurationMinutes = 525, Description = "SNTRI Line 189 Star Wars route", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("bbbbbbbb-2222-1111-1111-111111111111"), Name = "Tataouine - Tunis", Origin = "Tataouine", OriginCode = "TAT", Destination = "Tunis", DestinationCode = "TUN", DistanceKm = 530, EstimatedDurationMinutes = 525, Description = "SNTRI return Line 189", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("cccccccc-2222-1111-1111-111111111111"), Name = "Tunis - Ben Guarden", Origin = "Tunis", OriginCode = "TUN", Destination = "Ben Guarden", DestinationCode = "BGD", DistanceKm = 510, EstimatedDurationMinutes = 510, Description = "SNTRI Line 104 Libya border", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("dddddddd-2222-1111-1111-111111111111"), Name = "Ben Guarden - Tunis", Origin = "Ben Guarden", OriginCode = "BGD", Destination = "Tunis", DestinationCode = "TUN", DistanceKm = 510, EstimatedDurationMinutes = 510, Description = "SNTRI return Line 104", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("eeeeeeee-2222-1111-1111-111111111111"), Name = "Tunis - Matmata", Origin = "Tunis", OriginCode = "TUN", Destination = "Matmata", DestinationCode = "MAT", DistanceKm = 445, EstimatedDurationMinutes = 480, Description = "SNTRI Line 225 cave dwellings", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("ffffffff-2222-1111-1111-111111111111"), Name = "Matmata - Tunis", Origin = "Matmata", OriginCode = "MAT", Destination = "Tunis", DestinationCode = "TUN", DistanceKm = 445, EstimatedDurationMinutes = 480, Description = "SNTRI return Line 225", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            
            // Jerba Island Routes
            new Route { Id = Guid.Parse("11111111-3333-1111-1111-111111111111"), Name = "Tunis - Jerba", Origin = "Tunis", OriginCode = "TUN", Destination = "Houmt Souk", DestinationCode = "HOM", DistanceKm = 500, EstimatedDurationMinutes = 480, Description = "SNTRI Line 209 island destination", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("22222222-3333-1111-1111-111111111111"), Name = "Jerba - Tunis", Origin = "Houmt Souk", OriginCode = "HOM", Destination = "Tunis", DestinationCode = "TUN", DistanceKm = 500, EstimatedDurationMinutes = 480, Description = "SNTRI return Line 209", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("33333333-3333-1111-1111-111111111111"), Name = "Sfax - Jerba", Origin = "Sfax", OriginCode = "SFX", Destination = "Houmt Souk", DestinationCode = "HOM", DistanceKm = 230, EstimatedDurationMinutes = 210, Description = "SNTRI Line 164 southern route", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("44444444-3333-1111-1111-111111111111"), Name = "Jerba - Sfax", Origin = "Houmt Souk", OriginCode = "HOM", Destination = "Sfax", DestinationCode = "SFX", DistanceKm = 230, EstimatedDurationMinutes = 210, Description = "SNTRI return Line 164", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("55555555-3333-1111-1111-111111111111"), Name = "Sousse - Zarzis", Origin = "Sousse", OriginCode = "SUS", Destination = "Zarzis", DestinationCode = "ZAR", DistanceKm = 390, EstimatedDurationMinutes = 390, Description = "SNTRI Line 103 coastal south", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("66666666-3333-1111-1111-111111111111"), Name = "Zarzis - Sousse", Origin = "Zarzis", OriginCode = "ZAR", Destination = "Sousse", DestinationCode = "SUS", DistanceKm = 390, EstimatedDurationMinutes = 390, Description = "SNTRI return Line 103", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            
            // North-South Complete Routes
            new Route { Id = Guid.Parse("77777777-3333-1111-1111-111111111111"), Name = "Bizerte - Sousse", Origin = "Bizerte", OriginCode = "BIZ", Destination = "Sousse", DestinationCode = "SUS", DistanceKm = 205, EstimatedDurationMinutes = 240, Description = "SNTRI Line 154 north to coast", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("88888888-3333-1111-1111-111111111111"), Name = "Sousse - Bizerte", Origin = "Sousse", OriginCode = "SUS", Destination = "Bizerte", DestinationCode = "BIZ", DistanceKm = 205, EstimatedDurationMinutes = 240, Description = "SNTRI return Line 154", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("99999999-3333-1111-1111-111111111111"), Name = "Bizerte - Jerba", Origin = "Bizerte", OriginCode = "BIZ", Destination = "Houmt Souk", DestinationCode = "HOM", DistanceKm = 660, EstimatedDurationMinutes = 660, Description = "SNTRI Line 179 complete traverse", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
            new Route { Id = Guid.Parse("aaaaaaaa-3333-1111-1111-111111111111"), Name = "Jerba - Bizerte", Origin = "Houmt Souk", OriginCode = "HOM", Destination = "Bizerte", DestinationCode = "BIZ", DistanceKm = 660, EstimatedDurationMinutes = 660, Description = "SNTRI return Line 179", IsActive = true, CreatedAt = seedDate, IsDeleted = false },
        };
        modelBuilder.Entity<Route>().HasData(additionalRoutes);

        // ====== SNTRI REALISTIC SCHEDULES FOR NEXT 7 DAYS ======
        var schedules = new List<Schedule>();
        var scheduleId = 1000;

        // Generate SNTRI schedules with realistic frequencies and prices (in Tunisian Dinar TND)
        var routeScheduleConfigs = new List<(Guid RouteId, Guid BusId, Guid DriverId, decimal Price, int Hour, int[] Days)>
        {
            // ===== MAIN CORRIDOR: Tunis - Sfax (270km) - Multiple daily departures =====
            (Guid.Parse("55555555-5555-5555-5555-555555555555"), Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Guid.Parse("22222222-2222-2222-2222-222222222222"), 22.500m, 5, new[]{0,1,2,3,4,5,6}),  // Early morning
            (Guid.Parse("55555555-5555-5555-5555-555555555555"), Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), drivers[0].Id, 22.500m, 7, new[]{0,1,2,3,4,5,6}),  // Morning
            (Guid.Parse("55555555-5555-5555-5555-555555555555"), Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), drivers[1].Id, 22.500m, 10, new[]{0,1,2,3,4,5,6}), // Mid-morning
            (Guid.Parse("55555555-5555-5555-5555-555555555555"), additionalBuses[0].Id, drivers[3].Id, 22.500m, 14, new[]{0,1,2,3,4,5,6}), // Afternoon
            (Guid.Parse("55555555-5555-5555-5555-555555555555"), additionalBuses[1].Id, drivers[4].Id, 22.500m, 17, new[]{0,1,2,3,4,5,6}), // Evening
            
            // Sfax - Tunis (Return)
            (Guid.Parse("66666666-6666-6666-6666-666666666666"), Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), drivers[5].Id, 22.500m, 6, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("66666666-6666-6666-6666-666666666666"), additionalBuses[0].Id, drivers[0].Id, 22.500m, 9, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("66666666-6666-6666-6666-666666666666"), Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), drivers[1].Id, 22.500m, 13, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("66666666-6666-6666-6666-666666666666"), additionalBuses[1].Id, drivers[2].Id, 22.500m, 16, new[]{0,1,2,3,4,5,6}),
            
            // ===== TUNIS - SOUSSE (140km) - Very frequent service =====
            (Guid.Parse("77777777-7777-7777-7777-777777777777"), Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Guid.Parse("33333333-3333-3333-3333-333333333333"), 12.000m, 5, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("77777777-7777-7777-7777-777777777777"), Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), drivers[1].Id, 12.000m, 7, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("77777777-7777-7777-7777-777777777777"), additionalBuses[0].Id, drivers[0].Id, 12.000m, 9, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("77777777-7777-7777-7777-777777777777"), additionalBuses[1].Id, drivers[3].Id, 12.000m, 11, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("77777777-7777-7777-7777-777777777777"), Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), drivers[4].Id, 12.000m, 14, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("77777777-7777-7777-7777-777777777777"), additionalBuses[2].Id, drivers[5].Id, 12.000m, 16, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("77777777-7777-7777-7777-777777777777"), Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), drivers[6].Id, 12.000m, 19, new[]{0,1,2,3,4,5,6}),
            
            // Sousse - Tunis (Return)
            (Guid.Parse("88888888-8888-8888-8888-888888888888"), Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), drivers[0].Id, 12.000m, 6, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("88888888-8888-8888-8888-888888888888"), additionalBuses[1].Id, drivers[1].Id, 12.000m, 8, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("88888888-8888-8888-8888-888888888888"), Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), drivers[2].Id, 12.000m, 10, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("88888888-8888-8888-8888-888888888888"), additionalBuses[0].Id, drivers[3].Id, 12.000m, 13, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("88888888-8888-8888-8888-888888888888"), additionalBuses[2].Id, drivers[4].Id, 12.000m, 15, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("88888888-8888-8888-8888-888888888888"), Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), drivers[5].Id, 12.000m, 18, new[]{0,1,2,3,4,5,6}),
            
            // ===== SFAX - GABES (150km) =====
            (Guid.Parse("99999999-9999-9999-9999-999999999999"), additionalBuses[0].Id, drivers[0].Id, 13.500m, 6, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("99999999-9999-9999-9999-999999999999"), Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), drivers[1].Id, 13.500m, 11, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("99999999-9999-9999-9999-999999999999"), additionalBuses[1].Id, drivers[2].Id, 13.500m, 16, new[]{0,1,2,3,4,5,6}),
            
            // Gabes - Sfax (Return)
            (Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"), additionalBuses[0].Id, drivers[3].Id, 13.500m, 7, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"), Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), drivers[4].Id, 13.500m, 12, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"), additionalBuses[1].Id, drivers[5].Id, 13.500m, 17, new[]{0,1,2,3,4,5,6}),
            
            // ===== TUNIS - BIZERTE (70km) - Frequent northern route =====
            (Guid.Parse("bbbbbbbb-cccc-dddd-eeee-ffffffffffff"), Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), Guid.Parse("22222222-2222-2222-2222-222222222222"), 6.500m, 6, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("bbbbbbbb-cccc-dddd-eeee-ffffffffffff"), Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"), drivers[2].Id, 6.500m, 9, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("bbbbbbbb-cccc-dddd-eeee-ffffffffffff"), additionalBuses[2].Id, drivers[6].Id, 6.500m, 12, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("bbbbbbbb-cccc-dddd-eeee-ffffffffffff"), Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), Guid.Parse("33333333-3333-3333-3333-333333333333"), 6.500m, 15, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("bbbbbbbb-cccc-dddd-eeee-ffffffffffff"), Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"), drivers[2].Id, 6.500m, 18, new[]{0,1,2,3,4,5,6}),
            
            // Bizerte - Tunis (Return)
            (Guid.Parse("cccccccc-dddd-eeee-ffff-aaaaaaaaaaaa"), Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), drivers[0].Id, 6.500m, 7, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("cccccccc-dddd-eeee-ffff-aaaaaaaaaaaa"), Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"), drivers[1].Id, 6.500m, 10, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("cccccccc-dddd-eeee-ffff-aaaaaaaaaaaa"), additionalBuses[2].Id, drivers[2].Id, 6.500m, 13, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("cccccccc-dddd-eeee-ffff-aaaaaaaaaaaa"), Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), Guid.Parse("22222222-2222-2222-2222-222222222222"), 6.500m, 16, new[]{0,1,2,3,4,5,6}),
            
            // ===== CAP BON ROUTES - Tourist areas =====
            // Tunis - Nabeul (65km)
            (Guid.Parse("dddddddd-eeee-ffff-aaaa-bbbbbbbbbbbb"), additionalBuses[1].Id, drivers[1].Id, 6.000m, 7, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("dddddddd-eeee-ffff-aaaa-bbbbbbbbbbbb"), additionalBuses[0].Id, drivers[0].Id, 6.000m, 11, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("dddddddd-eeee-ffff-aaaa-bbbbbbbbbbbb"), Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), drivers[3].Id, 6.000m, 15, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("dddddddd-eeee-ffff-aaaa-bbbbbbbbbbbb"), additionalBuses[2].Id, drivers[4].Id, 6.000m, 18, new[]{0,1,2,3,4,5,6}),
            
            // Nabeul - Tunis (Return)
            (Guid.Parse("eeeeeeee-ffff-aaaa-bbbb-cccccccccccc"), additionalBuses[1].Id, drivers[5].Id, 6.000m, 6, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("eeeeeeee-ffff-aaaa-bbbb-cccccccccccc"), Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), drivers[6].Id, 6.000m, 10, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("eeeeeeee-ffff-aaaa-bbbb-cccccccccccc"), additionalBuses[0].Id, drivers[0].Id, 6.000m, 14, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("eeeeeeee-ffff-aaaa-bbbb-cccccccccccc"), additionalBuses[2].Id, drivers[1].Id, 6.000m, 17, new[]{0,1,2,3,4,5,6}),
            
            // Tunis - Hammamet (70km)
            (Guid.Parse("ffffffff-aaaa-bbbb-cccc-dddddddddddd"), Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), drivers[3].Id, 7.000m, 6, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("ffffffff-aaaa-bbbb-cccc-dddddddddddd"), Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), drivers[1].Id, 7.000m, 10, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("ffffffff-aaaa-bbbb-cccc-dddddddddddd"), additionalBuses[0].Id, drivers[2].Id, 7.000m, 14, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("ffffffff-aaaa-bbbb-cccc-dddddddddddd"), additionalBuses[1].Id, drivers[4].Id, 7.000m, 17, new[]{0,1,2,3,4,5,6}),
            
            // Hammamet - Tunis (Return)
            (Guid.Parse("aaaaaaaa-1111-2222-3333-444444444444"), Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), drivers[5].Id, 7.000m, 7, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("aaaaaaaa-1111-2222-3333-444444444444"), additionalBuses[0].Id, drivers[6].Id, 7.000m, 12, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("aaaaaaaa-1111-2222-3333-444444444444"), Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), drivers[0].Id, 7.000m, 16, new[]{0,1,2,3,4,5,6}),
            
            // Nabeul - Hammamet (15km) - Very frequent shuttle
            (Guid.Parse("bbbbbbbb-2222-3333-4444-555555555555"), Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"), drivers[2].Id, 2.000m, 8, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("bbbbbbbb-2222-3333-4444-555555555555"), additionalBuses[2].Id, drivers[3].Id, 2.000m, 11, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("bbbbbbbb-2222-3333-4444-555555555555"), Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), drivers[4].Id, 2.000m, 14, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("bbbbbbbb-2222-3333-4444-555555555555"), Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"), drivers[5].Id, 2.000m, 17, new[]{0,1,2,3,4,5,6}),
            
            // ===== SAHEL REGION =====
            // Sousse - Monastir (25km) - Frequent shuttle
            (Guid.Parse("cccccccc-3333-4444-5555-666666666666"), additionalBuses[1].Id, drivers[1].Id, 3.000m, 6, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("cccccccc-3333-4444-5555-666666666666"), additionalBuses[2].Id, drivers[2].Id, 3.000m, 9, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("cccccccc-3333-4444-5555-666666666666"), Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"), drivers[3].Id, 3.000m, 12, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("cccccccc-3333-4444-5555-666666666666"), additionalBuses[1].Id, drivers[4].Id, 3.000m, 15, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("cccccccc-3333-4444-5555-666666666666"), additionalBuses[2].Id, drivers[5].Id, 3.000m, 18, new[]{0,1,2,3,4,5,6}),
            
            // Monastir - Sousse (Return)
            (Guid.Parse("dddddddd-4444-5555-6666-777777777777"), additionalBuses[1].Id, drivers[6].Id, 3.000m, 7, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("dddddddd-4444-5555-6666-777777777777"), Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"), drivers[0].Id, 3.000m, 10, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("dddddddd-4444-5555-6666-777777777777"), additionalBuses[2].Id, drivers[1].Id, 3.000m, 13, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("dddddddd-4444-5555-6666-777777777777"), additionalBuses[1].Id, drivers[2].Id, 3.000m, 16, new[]{0,1,2,3,4,5,6}),
            
            // Sousse - Sfax (120km)
            (Guid.Parse("eeeeeeee-5555-6666-7777-888888888888"), Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), drivers[1].Id, 10.000m, 7, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("eeeeeeee-5555-6666-7777-888888888888"), additionalBuses[0].Id, drivers[3].Id, 10.000m, 13, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("eeeeeeee-5555-6666-7777-888888888888"), Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), drivers[4].Id, 10.000m, 17, new[]{0,1,2,3,4,5,6}),
            
            // Sfax - Sousse (Return)
            (Guid.Parse("ffffffff-6666-7777-8888-999999999999"), additionalBuses[0].Id, drivers[0].Id, 10.000m, 8, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("ffffffff-6666-7777-8888-999999999999"), Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), drivers[2].Id, 10.000m, 14, new[]{0,1,2,3,4,5,6}),
            
            // ===== CENTRAL ROUTES =====
            // Tunis - Kairouan (160km)
            (Guid.Parse("aaaaaaaa-7777-8888-9999-aaaaaaaaaaaa"), additionalBuses[0].Id, drivers[0].Id, 14.000m, 6, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("aaaaaaaa-7777-8888-9999-aaaaaaaaaaaa"), additionalBuses[1].Id, drivers[1].Id, 14.000m, 11, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("aaaaaaaa-7777-8888-9999-aaaaaaaaaaaa"), Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), drivers[3].Id, 14.000m, 16, new[]{0,1,2,3,4,5,6}),
            
            // Kairouan - Tunis (Return)
            (Guid.Parse("bbbbbbbb-8888-9999-aaaa-bbbbbbbbbbbb"), additionalBuses[0].Id, drivers[4].Id, 14.000m, 7, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("bbbbbbbb-8888-9999-aaaa-bbbbbbbbbbbb"), Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), drivers[5].Id, 14.000m, 13, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("bbbbbbbb-8888-9999-aaaa-bbbbbbbbbbbb"), additionalBuses[1].Id, drivers[6].Id, 14.000m, 17, new[]{0,1,2,3,4,5,6}),
            
            // Kairouan - Sousse (60km)
            (Guid.Parse("cccccccc-9999-aaaa-bbbb-cccccccccccc"), additionalBuses[2].Id, drivers[2].Id, 5.500m, 8, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("cccccccc-9999-aaaa-bbbb-cccccccccccc"), Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"), drivers[3].Id, 5.500m, 14, new[]{0,1,2,3,4,5,6}),
            
            // ===== DEEP SOUTH ROUTES =====
            // Gabes - Medenine (80km)
            (Guid.Parse("dddddddd-aaaa-bbbb-cccc-dddddddddddd"), additionalBuses[0].Id, drivers[0].Id, 7.500m, 7, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("dddddddd-aaaa-bbbb-cccc-dddddddddddd"), Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), drivers[1].Id, 7.500m, 13, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("dddddddd-aaaa-bbbb-cccc-dddddddddddd"), additionalBuses[1].Id, drivers[2].Id, 7.500m, 17, new[]{0,1,2,3,4,5,6}),
            
            // Medenine - Gabes (Return)
            (Guid.Parse("eeeeeeee-bbbb-cccc-dddd-eeeeeeeeeeee"), additionalBuses[0].Id, drivers[3].Id, 7.500m, 8, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("eeeeeeee-bbbb-cccc-dddd-eeeeeeeeeeee"), Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), drivers[4].Id, 7.500m, 14, new[]{0,1,2,3,4,5,6}),
            
            // Gabes - Tataouine (140km) - Desert route
            (Guid.Parse("ffffffff-cccc-dddd-eeee-ffffffffffff"), additionalBuses[0].Id, drivers[0].Id, 12.500m, 6, new[]{0,1,2,3,4,5}),  // Not Sunday
            (Guid.Parse("ffffffff-cccc-dddd-eeee-ffffffffffff"), Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), drivers[1].Id, 12.500m, 15, new[]{0,1,2,3,4,5}),
            
            // Tataouine - Gabes (Return)
            (Guid.Parse("aaaaaaaa-dddd-eeee-ffff-aaaaaaa11111"), additionalBuses[0].Id, drivers[2].Id, 12.500m, 7, new[]{0,1,2,3,4,5}),
            (Guid.Parse("aaaaaaaa-dddd-eeee-ffff-aaaaaaa11111"), Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), drivers[3].Id, 12.500m, 16, new[]{0,1,2,3,4,5}),
            
            // ===== EXPRESS LONG DISTANCE =====
            // Tunis - Gabes (400km) - Daily express
            (Guid.Parse("bbbbbbbb-eeee-ffff-aaaa-bbbbbbbb2222"), Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), drivers[5].Id, 35.000m, 5, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("bbbbbbbb-eeee-ffff-aaaa-bbbbbbbb2222"), additionalBuses[1].Id, drivers[6].Id, 35.000m, 14, new[]{0,1,2,3,4,5,6}),
            
            // Gabes - Tunis (Return Express)
            (Guid.Parse("cccccccc-ffff-aaaa-bbbb-cccccccc3333"), Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), drivers[0].Id, 35.000m, 6, new[]{0,1,2,3,4,5,6}),
            (Guid.Parse("cccccccc-ffff-aaaa-bbbb-cccccccc3333"), additionalBuses[1].Id, drivers[1].Id, 35.000m, 15, new[]{0,1,2,3,4,5,6}),
        };

        foreach (var config in routeScheduleConfigs)
        {
            for (int day = 0; day < 7; day++)
            {
                if (config.Days.Contains(day % 7))
                {
                    var departureDate = today.AddDays(day).AddHours(config.Hour);
                    var busCapacity = 45; // Default

                    schedules.Add(new Schedule
                    {
                        Id = Guid.NewGuid(),
                        BusId = config.BusId,
                        RouteId = config.RouteId,
                        DriverId = config.DriverId,
                        DepartureTime = departureDate,
                        ArrivalTime = departureDate.AddMinutes(GetRouteDuration(config.RouteId)),
                        BasePrice = config.Price,
                        AvailableSeats = busCapacity - (new Random(scheduleId).Next(0, 10)), // Random bookings
                        Status = day == 0 && config.Hour < now.Hour ? ScheduleStatus.InProgress : ScheduleStatus.Scheduled,
                        CurrentLatitude = day == 0 && config.Hour < now.Hour ? GetMidpointLat(config.RouteId) : null,
                        CurrentLongitude = day == 0 && config.Hour < now.Hour ? GetMidpointLon(config.RouteId) : null,
                        LastTrackingUpdate = day == 0 && config.Hour < now.Hour ? now.AddMinutes(-5) : null,
                        OperatingDaysJson = $"[{string.Join(",", config.Days)}]",
                        IsRecurring = true,
                        CreatedAt = seedDate,
                        IsDeleted = false
                    });
                    scheduleId++;
                }
            }
        }

        modelBuilder.Entity<Schedule>().HasData(schedules);
    }

    private static int GetRouteDuration(Guid routeId)
    {
        var durations = new Dictionary<Guid, int>
        {
            // Main routes
            { Guid.Parse("55555555-5555-5555-5555-555555555555"), 210 },  // Tunis - Sfax
            { Guid.Parse("66666666-6666-6666-6666-666666666666"), 210 },  // Sfax - Tunis
            { Guid.Parse("77777777-7777-7777-7777-777777777777"), 120 },  // Tunis - Sousse
            { Guid.Parse("88888888-8888-8888-8888-888888888888"), 120 },  // Sousse - Tunis
            { Guid.Parse("99999999-9999-9999-9999-999999999999"), 150 },  // Sfax - Gabes
            { Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"), 150 },  // Gabes - Sfax
            
            // Northern routes
            { Guid.Parse("bbbbbbbb-cccc-dddd-eeee-ffffffffffff"), 60 },   // Tunis - Bizerte
            { Guid.Parse("cccccccc-dddd-eeee-ffff-aaaaaaaaaaaa"), 60 },   // Bizerte - Tunis
            
            // Cap Bon
            { Guid.Parse("dddddddd-eeee-ffff-aaaa-bbbbbbbbbbbb"), 60 },   // Tunis - Nabeul
            { Guid.Parse("eeeeeeee-ffff-aaaa-bbbb-cccccccccccc"), 60 },   // Nabeul - Tunis
            { Guid.Parse("ffffffff-aaaa-bbbb-cccc-dddddddddddd"), 70 },   // Tunis - Hammamet
            { Guid.Parse("aaaaaaaa-1111-2222-3333-444444444444"), 70 },   // Hammamet - Tunis
            { Guid.Parse("bbbbbbbb-2222-3333-4444-555555555555"), 20 },   // Nabeul - Hammamet
            
            // Sahel
            { Guid.Parse("cccccccc-3333-4444-5555-666666666666"), 25 },   // Sousse - Monastir
            { Guid.Parse("dddddddd-4444-5555-6666-777777777777"), 25 },   // Monastir - Sousse
            { Guid.Parse("eeeeeeee-5555-6666-7777-888888888888"), 100 },  // Sousse - Sfax
            { Guid.Parse("ffffffff-6666-7777-8888-999999999999"), 100 },  // Sfax - Sousse
            
            // Central
            { Guid.Parse("aaaaaaaa-7777-8888-9999-aaaaaaaaaaaa"), 135 },  // Tunis - Kairouan
            { Guid.Parse("bbbbbbbb-8888-9999-aaaa-bbbbbbbbbbbb"), 135 },  // Kairouan - Tunis
            { Guid.Parse("cccccccc-9999-aaaa-bbbb-cccccccccccc"), 60 },   // Kairouan - Sousse
            
            // Deep South
            { Guid.Parse("dddddddd-aaaa-bbbb-cccc-dddddddddddd"), 75 },   // Gabes - Medenine
            { Guid.Parse("eeeeeeee-bbbb-cccc-dddd-eeeeeeeeeeee"), 75 },   // Medenine - Gabes
            { Guid.Parse("ffffffff-cccc-dddd-eeee-ffffffffffff"), 150 },  // Gabes - Tataouine
            { Guid.Parse("aaaaaaaa-dddd-eeee-ffff-aaaaaaa11111"), 150 },  // Tataouine - Gabes
            
            // Express long distance
            { Guid.Parse("bbbbbbbb-eeee-ffff-aaaa-bbbbbbbb2222"), 360 },  // Tunis - Gabes
            { Guid.Parse("cccccccc-ffff-aaaa-bbbb-cccccccc3333"), 360 },  // Gabes - Tunis
            
            // Legacy routes (keep for backward compatibility)
            { Guid.Parse("11111111-1111-1111-1111-111111111112"), 210 },
            { Guid.Parse("22222222-2222-2222-2222-222222222223"), 120 },
            { Guid.Parse("33333333-3333-3333-3333-333333333334"), 60 },
        };
        return durations.GetValueOrDefault(routeId, 120);
    }

    private static double? GetMidpointLat(Guid routeId)
    {
        var midpoints = new Dictionary<Guid, double>
        {
            // Main corridor midpoints (approximate GPS coordinates)
            { Guid.Parse("55555555-5555-5555-5555-555555555555"), 35.7735 },  // Tunis-Sfax midpoint
            { Guid.Parse("66666666-6666-6666-6666-666666666666"), 35.7735 },  // Sfax-Tunis midpoint
            { Guid.Parse("77777777-7777-7777-7777-777777777777"), 36.3161 },  // Tunis-Sousse midpoint
            { Guid.Parse("88888888-8888-8888-8888-888888888888"), 36.3161 },  // Sousse-Tunis midpoint
            { Guid.Parse("99999999-9999-9999-9999-999999999999"), 34.9500 },  // Sfax-Gabes midpoint
            { Guid.Parse("bbbbbbbb-cccc-dddd-eeee-ffffffffffff"), 37.0406 },  // Tunis-Bizerte midpoint
            { Guid.Parse("bbbbbbbb-eeee-ffff-aaaa-bbbbbbbb2222"), 35.5000 },  // Tunis-Gabes express
            
            // Legacy
            { Guid.Parse("11111111-1111-1111-1111-111111111112"), 35.7735 },
            { Guid.Parse("22222222-2222-2222-2222-222222222223"), 36.3161 },
            { Guid.Parse("33333333-3333-3333-3333-333333333334"), 37.0406 },
        };
        return midpoints.GetValueOrDefault(routeId);
    }

    private static double? GetMidpointLon(Guid routeId)
    {
        var midpoints = new Dictionary<Guid, double>
        {
            // Main corridor midpoints (approximate GPS coordinates)
            { Guid.Parse("55555555-5555-5555-5555-555555555555"), 10.4709 },  // Tunis-Sfax midpoint
            { Guid.Parse("66666666-6666-6666-6666-666666666666"), 10.4709 },  // Sfax-Tunis midpoint
            { Guid.Parse("77777777-7777-7777-7777-777777777777"), 10.4113 },  // Tunis-Sousse midpoint
            { Guid.Parse("88888888-8888-8888-8888-888888888888"), 10.4113 },  // Sousse-Tunis midpoint
            { Guid.Parse("99999999-9999-9999-9999-999999999999"), 10.3000 },  // Sfax-Gabes midpoint
            { Guid.Parse("bbbbbbbb-cccc-dddd-eeee-ffffffffffff"), 10.0277 },  // Tunis-Bizerte midpoint
            { Guid.Parse("bbbbbbbb-eeee-ffff-aaaa-bbbbbbbb2222"), 10.2500 },  // Tunis-Gabes express
            
            // Legacy
            { Guid.Parse("11111111-1111-1111-1111-111111111112"), 10.4709 },
            { Guid.Parse("22222222-2222-2222-2222-222222222223"), 10.4113 },
            { Guid.Parse("33333333-3333-3333-3333-333333333334"), 10.0277 },
        };
        return midpoints.GetValueOrDefault(routeId);
    }
}
