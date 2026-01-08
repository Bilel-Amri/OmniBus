using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmniBus.Domain.Entities;

namespace OmniBus.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for User
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        
        builder.HasKey(u => u.Id);
        
        builder.HasIndex(u => u.Email)
            .IsUnique();
        
        builder.HasIndex(u => u.NationalId)
            .IsUnique();
        
        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(256);
        
        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(20);
        
        builder.Property(u => u.ProfilePictureUrl)
            .HasMaxLength(500);
        
        builder.Property(u => u.NationalId)
            .HasMaxLength(20);
        
        // Relationships
        builder.HasMany(u => u.Tickets)
            .WithOne(t => t.User)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(u => u.AssignedBus)
            .WithOne()
            .HasForeignKey<User>(u => u.AssignedBusId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

/// <summary>
/// Entity configuration for Bus
/// </summary>
public class BusConfiguration : IEntityTypeConfiguration<Bus>
{
    public void Configure(EntityTypeBuilder<Bus> builder)
    {
        builder.ToTable("Buses");
        
        builder.HasKey(b => b.Id);
        
        builder.HasIndex(b => b.PlateNumber)
            .IsUnique();
        
        builder.HasIndex(b => b.RegistrationNumber)
            .IsUnique();
        
        builder.Property(b => b.PlateNumber)
            .IsRequired()
            .HasMaxLength(20);
        
        builder.Property(b => b.RegistrationNumber)
            .IsRequired()
            .HasMaxLength(20);
        
        builder.Property(b => b.Brand)
            .HasMaxLength(50);
        
        builder.Property(b => b.Model)
            .HasMaxLength(50);
        
        // Relationships
        builder.HasMany(b => b.Schedules)
            .WithOne(s => s.Bus)
            .HasForeignKey(s => s.BusId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(b => b.CurrentDriver)
            .WithMany()
            .HasForeignKey(b => b.CurrentDriverId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

/// <summary>
/// Entity configuration for Route
/// </summary>
public class RouteConfiguration : IEntityTypeConfiguration<Route>
{
    public void Configure(EntityTypeBuilder<Route> builder)
    {
        builder.ToTable("Routes");
        
        builder.HasKey(r => r.Id);
        
        builder.HasIndex(r => r.Name);
        
        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(r => r.Origin)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(r => r.OriginCode)
            .HasMaxLength(10);
        
        builder.Property(r => r.Destination)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(r => r.DestinationCode)
            .HasMaxLength(10);
        
        builder.Property(r => r.Description)
            .HasMaxLength(500);
        
        // Relationships
        builder.HasMany(r => r.Schedules)
            .WithOne(s => s.Route)
            .HasForeignKey(s => s.RouteId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(r => r.Stops)
            .WithOne(s => s.Route)
            .HasForeignKey(s => s.RouteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>
/// Entity configuration for RouteStop
/// </summary>
public class RouteStopConfiguration : IEntityTypeConfiguration<RouteStop>
{
    public void Configure(EntityTypeBuilder<RouteStop> builder)
    {
        builder.ToTable("RouteStops");
        
        builder.HasKey(rs => rs.Id);
        
        builder.Property(rs => rs.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(rs => rs.Order)
            .IsRequired();
    }
}

/// <summary>
/// Entity configuration for Schedule
/// </summary>
public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> builder)
    {
        builder.ToTable("Schedules");
        
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.DepartureTime)
            .IsRequired();
        
        builder.Property(s => s.ArrivalTime)
            .IsRequired();
        
        builder.Property(s => s.BasePrice)
            .HasColumnType("decimal(10,2)")
            .IsRequired();
        
        builder.Property(s => s.AvailableSeats)
            .IsRequired();
        
        builder.Property(s => s.DelayReason)
            .HasMaxLength(500);
        
        // Relationships
        builder.HasOne(s => s.Bus)
            .WithMany(b => b.Schedules)
            .HasForeignKey(s => s.BusId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(s => s.Route)
            .WithMany(r => r.Schedules)
            .HasForeignKey(s => s.RouteId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(s => s.Driver)
            .WithMany()
            .HasForeignKey(s => s.DriverId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.HasMany(s => s.Tickets)
            .WithOne(t => t.Schedule)
            .HasForeignKey(t => t.ScheduleId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(s => s.SeatLocks)
            .WithOne(sl => sl.Schedule)
            .HasForeignKey(sl => sl.ScheduleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>
/// Entity configuration for Ticket
/// </summary>
public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("Tickets");
        
        builder.HasKey(t => t.Id);
        
        builder.HasIndex(t => t.BookingReference)
            .IsUnique();
        
        builder.Property(t => t.BookingReference)
            .IsRequired()
            .HasMaxLength(20);
        
        builder.Property(t => t.Price)
            .HasColumnType("decimal(10,2)")
            .IsRequired();
        
        builder.Property(t => t.PassengerName)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(t => t.PassengerPhone)
            .HasMaxLength(20);
        
        builder.Property(t => t.CancellationReason)
            .HasMaxLength(500);
        
        builder.Property(t => t.QrCode);
        
        // Relationships
        builder.HasOne(t => t.User)
            .WithMany(u => u.Tickets)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(t => t.Schedule)
            .WithMany(s => s.Tickets)
            .HasForeignKey(t => t.ScheduleId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(t => t.Payment)
            .WithOne(p => p.Ticket)
            .HasForeignKey<Ticket>(t => t.PaymentId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

/// <summary>
/// Entity configuration for Payment
/// </summary>
public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");
        
        builder.HasKey(p => p.Id);
        
        builder.HasIndex(p => p.TransactionId)
            .IsUnique();
        
        builder.Property(p => p.Amount)
            .HasColumnType("decimal(10,2)")
            .IsRequired();
        
        builder.Property(p => p.Currency)
            .HasMaxLength(3)
            .HasDefaultValue("TND");
        
        builder.Property(p => p.TransactionId)
            .HasMaxLength(100);
        
        builder.Property(p => p.GatewayResponse)
            .HasMaxLength(2000);
        
        builder.Property(p => p.FailureReason)
            .HasMaxLength(500);
        
        // Relationships
        builder.HasOne(p => p.Ticket)
            .WithOne(t => t.Payment)
            .HasForeignKey<Payment>(p => p.TicketId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>
/// Entity configuration for SeatLock
/// </summary>
public class SeatLockConfiguration : IEntityTypeConfiguration<SeatLock>
{
    public void Configure(EntityTypeBuilder<SeatLock> builder)
    {
        builder.ToTable("SeatLocks");
        
        builder.HasKey(sl => sl.Id);
        
        builder.HasIndex(sl => new { sl.ScheduleId, sl.SeatNumber });
        
        builder.Property(sl => sl.SessionId)
            .IsRequired()
            .HasMaxLength(100);
        
        // Relationships
        builder.HasOne(sl => sl.Schedule)
            .WithMany(s => s.SeatLocks)
            .HasForeignKey(sl => sl.ScheduleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
