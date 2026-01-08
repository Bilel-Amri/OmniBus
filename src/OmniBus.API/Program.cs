using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OmniBus.Application.Common.Mappings;
using OmniBus.Application.Interfaces;
using OmniBus.Domain.Interfaces;
using OmniBus.Infrastructure.Persistence;
using OmniBus.Infrastructure.Repositories;
using OmniBus.Infrastructure.Services;
using OmniBus.API.Hubs;
using OmniBus.API.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure PostgreSQL Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBusService, BusService>();
builder.Services.AddScoped<IRouteService, RouteService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IDriverService, DriverService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHttpClient<IAIAssistantService, DifyAIService>();
builder.Services.AddHttpClient<IPaymentGatewayService, D17PaymentGatewayService>();

// Add SignalR
builder.Services.AddSignalR();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Add Controllers
builder.Services.AddControllers();

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"];
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "OmniBus API", 
        Version = "v1",
        Description = "Bus Service Application API for Tunisia"
    });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
    
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Use error handling middleware
app.UseErrorHandling();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Map SignalR hubs
app.MapHub<TrackingHub>("/hubs/tracking");
app.MapHub<SmartTrackingHub>("/hubs/smart-tracking"); // Smart throttling GPS tracking
app.MapHub<BookingHub>("/hubs/booking");

// Seed database on startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try 
    {
        // Drop and recreate database to match updated schema
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        
        // Add basic seed data if database is empty
        if (!context.Users.Any())
        {
            var seedDate = DateTime.UtcNow;
            
            // Add admin user
            context.Users.Add(new OmniBus.Domain.Entities.User 
            {
                Id = Guid.NewGuid(),
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@omnibus.tn",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = OmniBus.Domain.Enums.UserRole.Admin,
                PhoneNumber = "+216 98 000 000",
                EmailVerified = true,
                CreatedAt = seedDate,
                IsDeleted = false
            });
            
            // Add sample driver
            var driverId = Guid.NewGuid();
            context.Users.Add(new OmniBus.Domain.Entities.User 
            {
                Id = driverId,
                FirstName = "Mohamed",
                LastName = "Ben Ali",
                Email = "driver@omnibus.tn",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Driver123!"),
                Role = OmniBus.Domain.Enums.UserRole.Driver,
                PhoneNumber = "+216 98 123 456",
                EmailVerified = true,
                CreatedAt = seedDate,
                IsDeleted = false
            });
            
            // Add sample passenger
            context.Users.Add(new OmniBus.Domain.Entities.User 
            {
                Id = Guid.NewGuid(),
                FirstName = "Fatma",
                LastName = "Zahra",
                Email = "passenger@omnibus.tn",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pass123!"),
                Role = OmniBus.Domain.Enums.UserRole.Passenger,
                PhoneNumber = "+216 92 555 777",
                EmailVerified = true,
                CreatedAt = seedDate,
                IsDeleted = false
            });
            
            // Add sample bus
            var busId = Guid.NewGuid();
            context.Buses.Add(new OmniBus.Domain.Entities.Bus
            {
                Id = busId,
                PlateNumber = "TN 1234",
                RegistrationNumber = "REG-001",
                Brand = "Mercedes",
                Model = "Travego",
                YearManufactured = 2022,
                Capacity = 50,
                AvailableSeats = 50,
                Type = OmniBus.Domain.Enums.BusType.Intercity,
                Status = OmniBus.Domain.Enums.BusStatus.Active,
                SeatsPerRow = 4,
                HasAirConditioning = true,
                HasWifi = true,
                IsWheelchairAccessible = false,
                CurrentDriverId = driverId,
                CurrentLatitude = 36.8065,
                CurrentLongitude = 10.1815,
                LastLocationUpdate = DateTime.UtcNow,
                CreatedAt = seedDate,
                IsDeleted = false
            });
            
            // Add sample route
            var routeId = Guid.NewGuid();
            context.Routes.Add(new OmniBus.Domain.Entities.Route
            {
                Id = routeId,
                Name = "Tunis - Sfax Express",
                Origin = "Tunis",
                OriginCode = "TUN",
                Destination = "Sfax",
                DestinationCode = "SFX",
                DistanceKm = 270,
                EstimatedDurationMinutes = 210,
                Description = "Express service between Tunis and Sfax",
                IsActive = true,
                CreatedAt = seedDate,
                IsDeleted = false
            });
            
            // Add sample schedule (today + 1 day)
            var scheduleDate = DateTime.UtcNow.AddDays(1);
            context.Schedules.Add(new OmniBus.Domain.Entities.Schedule
            {
                Id = Guid.NewGuid(),
                RouteId = routeId,
                BusId = busId,
                DriverId = driverId,
                DepartureTime = scheduleDate.Date.AddHours(8),
                ArrivalTime = scheduleDate.Date.AddHours(11).AddMinutes(30),
                Status = OmniBus.Domain.Enums.ScheduleStatus.Scheduled,
                BasePrice = 25.00m,
                AvailableSeats = 50,
                CreatedAt = seedDate,
                IsDeleted = false
            });
            
            context.SaveChanges();
            Console.WriteLine("✅ Database seeded with sample data");
        }
        else
        {
            Console.WriteLine("ℹ️  Database already contains data");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Database seeding error: {ex.Message}");
        // Continue anyway - API will still run
    }
}

app.Run();

// Make Program class accessible for testing
public partial class Program { }
