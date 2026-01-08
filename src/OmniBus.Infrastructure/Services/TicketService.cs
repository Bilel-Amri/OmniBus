using AutoMapper;
using OmniBus.Application.DTOs;
using OmniBus.Application.Interfaces;
using OmniBus.Domain.Entities;
using OmniBus.Domain.Enums;
using OmniBus.Domain.Interfaces;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

namespace OmniBus.Infrastructure.Services;

/// <summary>
/// Ticket service implementation
/// </summary>
public class TicketService : ITicketService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;
    
    public TicketService(IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _emailService = emailService;
    }
    
    public async Task<IEnumerable<TicketResponseDto>> GetAllTicketsAsync()
    {
        var tickets = await _unitOfWork.Tickets.GetAllAsync();
        return tickets.Select(MapToResponse).ToList();
    }
    
    public async Task<TicketResponseDto?> GetTicketByIdAsync(Guid id)
    {
        var ticket = await _unitOfWork.Tickets.GetByIdAsync(id);
        if (ticket == null) return null;
        return MapToResponse(ticket);
    }
    
    public async Task<TicketResponseDto?> GetTicketByReferenceAsync(string reference)
    {
        var ticket = await _unitOfWork.Tickets.GetByBookingReferenceAsync(reference);
        if (ticket == null) return null;
        return MapToResponse(ticket);
    }
    
    public async Task<IEnumerable<MyTicketDto>> GetTicketsByUserAsync(Guid userId)
    {
        var tickets = await _unitOfWork.Tickets.GetByUserAsync(userId);
        return tickets.Select(MapToMyTicket).ToList();
    }
    
    public async Task<IEnumerable<MyTicketDto>> GetUpcomingTicketsByUserAsync(Guid userId)
    {
        var tickets = await _unitOfWork.Tickets.GetUpcomingByUserAsync(userId);
        return tickets.Select(MapToMyTicket).ToList();
    }
    
    public async Task<IEnumerable<MyTicketDto>> GetCompletedTicketsByUserAsync(Guid userId)
    {
        var tickets = await _unitOfWork.Tickets.GetCompletedByUserAsync(userId);
        return tickets.Select(MapToMyTicket).ToList();
    }
    
    public async Task<SeatLockResponseDto?> LockSeatAsync(Guid userId, SeatLockRequestDto request, string sessionId)
    {
        var schedule = await _unitOfWork.Schedules.GetByIdAsync(request.ScheduleId);
        if (schedule == null || schedule.AvailableSeats <= 0)
            return null;
        
        // Check if seat is already booked
        if (await _unitOfWork.Tickets.IsSeatBookedAsync(request.ScheduleId, request.SeatNumber))
            return null;
        
        // Check if seat is already locked by another user
        var existingLock = await _unitOfWork.SeatLocks.GetActiveLockAsync(request.ScheduleId, request.SeatNumber);
        if (existingLock != null && existingLock.UserId != userId)
            return null;
        
        // Check if user already has a lock on this seat
        var userLock = await _unitOfWork.SeatLocks.GetUserActiveLockAsync(request.ScheduleId, userId);
        if (userLock != null && userLock.SeatNumber != request.SeatNumber)
        {
            // Release old lock
            userLock.Status = SeatLockStatus.Available;
            await _unitOfWork.SeatLocks.UpdateAsync(userLock);
        }
        
        // Create new lock
        var lockExpiry = DateTime.UtcNow.AddMinutes(5); // 5 minute lock
        var seatLock = new SeatLock
        {
            ScheduleId = request.ScheduleId,
            UserId = userId,
            SeatNumber = request.SeatNumber,
            LockedAt = DateTime.UtcNow,
            ExpiresAt = lockExpiry,
            Status = SeatLockStatus.Locked,
            SessionId = sessionId
        };
        
        await _unitOfWork.SeatLocks.CreateAsync(seatLock);
        
        return new SeatLockResponseDto
        {
            LockId = seatLock.Id,
            ScheduleId = request.ScheduleId,
            SeatNumber = request.SeatNumber,
            ExpiresAt = lockExpiry,
            DurationSeconds = 300 // 5 minutes
        };
    }
    
    public async Task<bool> ReleaseSeatLockAsync(Guid lockId)
    {
        var lockItem = await _unitOfWork.SeatLocks.GetByIdAsync(lockId);
        if (lockItem == null) return false;
        
        lockItem.Status = SeatLockStatus.Available;
        await _unitOfWork.SeatLocks.UpdateAsync(lockItem);
        
        return true;
    }
    
    public async Task<bool> ReleaseUserLocksAsync(Guid userId, string sessionId)
    {
        await _unitOfWork.SeatLocks.ReleaseUserLocksAsync(userId, sessionId);
        return true;
    }
    
    public async Task<TicketResponseDto> BookTicketAsync(Guid userId, CreateTicketDto request)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            throw new Exception("User not found");
        
        var schedule = await _unitOfWork.Schedules.GetByIdAsync(request.ScheduleId);
        if (schedule == null)
            throw new Exception("Schedule not found");
        
        if (schedule.Route == null || schedule.Bus == null)
            throw new Exception("Schedule data is incomplete. Route or Bus information is missing.");
        
        if (schedule.Status == ScheduleStatus.Cancelled)
            throw new Exception("This trip has been cancelled");
        
        if (schedule.AvailableSeats <= 0)
            throw new Exception("No seats available");
        
        // Check if seat is booked
        if (await _unitOfWork.Tickets.IsSeatBookedAsync(request.ScheduleId, request.SeatNumber))
            throw new Exception("This seat is already booked");
        
        // Check if seat is locked by another user
        var existingLock = await _unitOfWork.SeatLocks.GetActiveLockAsync(request.ScheduleId, request.SeatNumber);
        if (existingLock != null && existingLock.UserId != userId)
            throw new Exception("This seat is currently being booked by another user");
        
        // Create ticket
        var ticket = new Ticket
        {
            UserId = userId,
            ScheduleId = request.ScheduleId,
            SeatNumber = request.SeatNumber,
            Status = TicketStatus.Reserved,
            Price = schedule.BasePrice,
            PassengerName = request.PassengerName,
            PassengerPhone = request.PassengerPhone,
            QrCode = GenerateQrCode(userId, request.ScheduleId, request.SeatNumber)
        };
        
        await _unitOfWork.Tickets.CreateAsync(ticket);
        
        // Update available seats
        await _unitOfWork.Schedules.DecrementAvailableSeatsAsync(request.ScheduleId);
        
        // Release seat lock if exists
        if (existingLock != null)
        {
            existingLock.Status = SeatLockStatus.Booked;
            await _unitOfWork.SeatLocks.UpdateAsync(existingLock);
        }
        
        await _unitOfWork.SaveChangesAsync();
        
        // Attach schedule for response mapping
        ticket.Schedule = schedule;
        
        // Send booking confirmation email
        var routeInfo = $"{schedule.Route.Origin} → {schedule.Route.Destination}";
        _ = _emailService.SendBookingConfirmationAsync(
            user.Email,
            user.FirstName + " " + user.LastName,
            ticket.BookingReference,
            routeInfo,
            schedule.DepartureTime,
            ticket.Price
        );
        
        return MapToResponse(ticket);
    }
    
    public async Task<TicketResponseDto?> CancelTicketAsync(Guid ticketId, CancelTicketDto request)
    {
        var ticket = await _unitOfWork.Tickets.GetByIdAsync(ticketId);
        if (ticket == null) return null;
        
        if (ticket.Status == TicketStatus.Cancelled)
            throw new Exception("Ticket is already cancelled");
        
        if (ticket.Status == TicketStatus.Completed)
            throw new Exception("Cannot cancel a used ticket");
        
        // Check if trip has already departed
        if (ticket.Schedule.DepartureTime < DateTime.UtcNow)
            throw new Exception("Cannot cancel a ticket after departure");
        
        ticket.Status = TicketStatus.Cancelled;
        ticket.CancellationReason = request.Reason;
        ticket.CancelledAt = DateTime.UtcNow;
        
        await _unitOfWork.Tickets.UpdateAsync(ticket);
        
        // Restore available seats
        await _unitOfWork.Schedules.IncrementAvailableSeatsAsync(ticket.ScheduleId);
        
        await _unitOfWork.SaveChangesAsync();
        
        // Send cancellation notification email
        var user = await _unitOfWork.Users.GetByIdAsync(ticket.UserId);
        if (user != null)
        {
            var routeInfo = $"{ticket.Schedule.Route.Origin} → {ticket.Schedule.Route.Destination}";
            _ = _emailService.SendCancellationNotificationAsync(
                user.Email,
                user.FirstName + " " + user.LastName,
                ticket.BookingReference,
                routeInfo
            );
        }
        
        return MapToResponse(ticket);
    }
    
    public async Task<bool> ConfirmBoardingAsync(Guid ticketId, Guid driverId, string? qrCode)
    {
        var ticket = await _unitOfWork.Tickets.GetByIdAsync(ticketId);
        if (ticket == null) return false;
        
        if (ticket.Status != TicketStatus.Booked)
            return false;
        
        ticket.Status = TicketStatus.Completed;
        ticket.BoardingTime = DateTime.UtcNow;
        ticket.BoardedBy = driverId.ToString();
        
        await _unitOfWork.Tickets.UpdateAsync(ticket);
        await _unitOfWork.SaveChangesAsync();
        
        return true;
    }
    
    public async Task<IEnumerable<int>> GetBookedSeatsAsync(Guid scheduleId)
    {
        return await _unitOfWork.Tickets.GetBookedSeatsAsync(scheduleId);
    }
    
    public async Task<ScheduleSeatLayoutDto?> GetSeatLayoutWithAvailabilityAsync(Guid scheduleId, Guid? userId = null)
    {
        var schedule = await _unitOfWork.Schedules.GetByIdAsync(scheduleId);
        if (schedule == null) return null;
        
        var layout = new ScheduleSeatLayoutDto
        {
            ScheduleId = schedule.Id,
            DepartureTime = schedule.DepartureTime,
            Origin = schedule.Route.Origin,
            Destination = schedule.Route.Destination,
            TotalSeats = schedule.Bus.Capacity,
            SeatsPerRow = schedule.Bus.SeatsPerRow,
            Price = schedule.BasePrice
        };
        
        // Get booked seats
        var bookedSeats = await _unitOfWork.Tickets.GetBookedSeatsAsync(scheduleId);
        var bookedSeatsSet = bookedSeats.ToHashSet();
        
        // Get locked seats
        var lockedLocks = await _unitOfWork.SeatLocks.GetScheduleLocksAsync(scheduleId);
        var lockedSeats = lockedLocks.Where(l => l.IsValid).Select(l => l.SeatNumber).ToHashSet();
        
        // Generate seat statuses
        for (int seat = 1; seat <= schedule.Bus.Capacity; seat++)
        {
            var status = new SeatStatusDto { SeatNumber = seat };
            
            if (bookedSeatsSet.Contains(seat))
                status.Status = "Booked";
            else if (lockedSeats.Contains(seat))
                status.Status = "Locked";
            else
                status.Status = "Available";
            
            layout.Seats.Add(status);
        }
        
        return layout;
    }
    
    public async Task<TicketStatsDto> GetTicketStatsAsync()
    {
        var allTickets = await _unitOfWork.Tickets.GetAllAsync();
        var today = DateTime.UtcNow.Date;
        var todayEnd = today.AddDays(1);
        
        var todayTickets = allTickets.Where(t => t.CreatedAt >= today && t.CreatedAt < todayEnd);
        
        var stats = new TicketStatsDto
        {
            TotalTickets = allTickets.Count(),
            CompletedTickets = allTickets.Count(t => t.Status == TicketStatus.Completed),
            CancelledTickets = allTickets.Count(t => t.Status == TicketStatus.Cancelled),
            PendingTickets = allTickets.Count(t => t.Status == TicketStatus.Reserved),
            TotalRevenue = allTickets.Where(t => t.Status == TicketStatus.Booked || t.Status == TicketStatus.Completed)
                .Sum(t => t.Price),
            TodayRevenue = todayTickets.Where(t => t.Status == TicketStatus.Booked || t.Status == TicketStatus.Completed)
                .Sum(t => t.Price)
        };
        
        return stats;
    }
    
    private TicketResponseDto MapToResponse(Ticket ticket)
    {
        if (ticket.Schedule == null)
            throw new InvalidOperationException("Ticket schedule is not loaded");
        if (ticket.Schedule.Route == null)
            throw new InvalidOperationException("Schedule route is not loaded");
        if (ticket.Schedule.Bus == null)
            throw new InvalidOperationException("Schedule bus is not loaded");
            
        return new TicketResponseDto
        {
            Id = ticket.Id,
            BookingReference = ticket.BookingReference,
            SeatNumber = ticket.SeatNumber,
            Status = ticket.Status.ToString(),
            Price = ticket.Price,
            BookingDate = ticket.BookingDate,
            QrCode = ticket.QrCode,
            PassengerName = ticket.PassengerName,
            Schedule = new ScheduleSummaryDto
            {
                Id = ticket.Schedule.Id,
                DepartureTime = ticket.Schedule.DepartureTime,
                ArrivalTime = ticket.Schedule.ArrivalTime,
                Origin = ticket.Schedule.Route.Origin,
                Destination = ticket.Schedule.Route.Destination,
                BusPlateNumber = ticket.Schedule.Bus.PlateNumber,
                BusType = ticket.Schedule.Bus.Type.ToString()
            }
        };
    }
    
    private MyTicketDto MapToMyTicket(Ticket ticket)
    {
        return new MyTicketDto
        {
            Id = ticket.Id,
            BookingReference = ticket.BookingReference,
            SeatNumber = ticket.SeatNumber,
            Status = ticket.Status.ToString(),
            Price = ticket.Price,
            BookingDate = ticket.BookingDate,
            QrCode = ticket.QrCode,
            DepartureTime = ticket.Schedule.DepartureTime,
            Origin = ticket.Schedule.Route.Origin,
            Destination = ticket.Schedule.Route.Destination,
            BusType = ticket.Schedule.Bus.Type.ToString(),
            CancellationReason = ticket.CancellationReason
        };
    }
    
    private string GenerateQrCode(Guid userId, Guid scheduleId, int seatNumber)
    {
        var data = $"OMNIBUS|{userId}|{scheduleId}|{seatNumber}|{DateTime.UtcNow:yyyyMMddHHmmss}";
        
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        var qrBytes = qrCode.GetGraphic(20);
        
        return $"data:image/png;base64,{Convert.ToBase64String(qrBytes)}";
    }
}
