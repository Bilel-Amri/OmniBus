using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OmniBus.Application.Interfaces;
using OmniBus.Domain.Interfaces;
using OmniBus.Infrastructure.Persistence;
using OmniBus.Infrastructure.Repositories;
using OmniBus.Infrastructure.Services;
using StackExchange.Redis;

namespace OmniBus.Infrastructure;

/// <summary>
/// Dependency injection extensions for Infrastructure layer
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Add infrastructure services to the DI container
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Register DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            // Configuration is done in Program.cs
        });
        
        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // Register Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBusRepository, BusRepository>();
        services.AddScoped<IRouteRepository, RouteRepository>();
        services.AddScoped<IScheduleRepository, ScheduleRepository>();
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<ISeatLockRepository, SeatLockRepository>();
        
        // Register Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IBusService, BusService>();
        services.AddScoped<IRouteService, RouteService>();
        services.AddScoped<IScheduleService, ScheduleService>();
        services.AddScoped<ITicketService, TicketService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IDriverService, DriverService>();
        services.AddScoped<IAnalyticsService, AnalyticsService>();
        
        // Register AI Services
        services.AddHttpClient<IAIAssistantService, DifyAIService>();
        
        // Register Email Service
        services.AddScoped<IEmailService, EmailService>();
        
        // Register Payment Gateway Services
        services.AddHttpClient<IPaymentGatewayService, D17PaymentGatewayService>();
        services.AddHttpClient<IMultiPaymentGateway, MultiPaymentGatewayService>();
        
        // Register SMS Service
        services.AddHttpClient<ISmsService, TunisieSmsService>();
        
        // Register Redis for high-concurrency seat locking
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var redisConnection = configuration["Redis:ConnectionString"] ?? "localhost:6379";
            return ConnectionMultiplexer.Connect(redisConnection);
        });
        services.AddScoped<IRedisLockService, RedisLockService>();
        
        // Register QR Code Service
        services.AddScoped<IQrCodeService, SecureQrCodeService>();
        
        return services;
    }
}
