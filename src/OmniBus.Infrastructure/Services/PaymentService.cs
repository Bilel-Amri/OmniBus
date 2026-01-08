using AutoMapper;
using OmniBus.Application.DTOs;
using OmniBus.Application.Interfaces;
using OmniBus.Domain.Entities;
using OmniBus.Domain.Enums;
using OmniBus.Domain.Interfaces;

namespace OmniBus.Infrastructure.Services;

/// <summary>
/// Payment service implementation
/// </summary>
public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPaymentGatewayService _paymentGateway;
    private readonly IEmailService _emailService;
    
    public PaymentService(IUnitOfWork unitOfWork, IMapper mapper, IPaymentGatewayService paymentGateway, IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _paymentGateway = paymentGateway;
        _emailService = emailService;
    }
    
    public async Task<PaymentResultDto> ProcessPaymentAsync(Guid userId, CreatePaymentDto request)
    {
        var ticket = await _unitOfWork.Tickets.GetByIdAsync(request.TicketId);
        if (ticket == null)
        {
            return new PaymentResultDto
            {
                Success = false,
                ErrorMessage = "Ticket not found"
            };
        }
        
        if (ticket.UserId != userId)
        {
            return new PaymentResultDto
            {
                Success = false,
                ErrorMessage = "You are not authorized to pay for this ticket"
            };
        }
        
        if (ticket.Status == TicketStatus.Cancelled)
        {
            return new PaymentResultDto
            {
                Success = false,
                ErrorMessage = "This ticket has been cancelled"
            };
        }
        
        if (ticket.Status == TicketStatus.Booked && ticket.PaymentId.HasValue)
        {
            return new PaymentResultDto
            {
                Success = false,
                ErrorMessage = "This ticket has already been paid"
            };
        }
        
        // Initiate payment through D17 gateway
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        var paymentResult = await _paymentGateway.InitiatePaymentAsync(
            ticket.Price,
            "TND",
            ticket.BookingReference,
            user?.Email ?? ""
        );
        
        if (!paymentResult.Success)
        {
            return new PaymentResultDto
            {
                Success = false,
                ErrorMessage = paymentResult.ErrorMessage ?? "Payment initiation failed"
            };
        }
        
        var payment = new Payment
        {
            TicketId = request.TicketId,
            UserId = userId,
            Amount = ticket.Price,
            Currency = "TND",
            Status = PaymentStatus.Pending,
            PaymentMethod = request.PaymentMethod,
            TransactionId = paymentResult.TransactionId ?? $"TXN-{Guid.NewGuid():N}".Substring(0, 16).ToUpper()
        };
        
        await _unitOfWork.Payments.CreateAsync(payment);
        await _unitOfWork.SaveChangesAsync();
        
        // Send payment confirmation email after successful payment
        if (user != null && payment.Status == PaymentStatus.Completed)
        {
            _ = _emailService.SendPaymentConfirmationAsync(
                user.Email,
                user.FirstName + " " + user.LastName,
                ticket.BookingReference,
                payment.Amount,
                payment.TransactionId
            );
        }
        
        return new PaymentResultDto
        {
            Success = true,
            TransactionId = payment.TransactionId,
            PaymentUrl = paymentResult.PaymentUrl,
            Payment = _mapper.Map<PaymentResponseDto>(payment)
        };
    }
    
    public async Task<bool> HandlePaymentCallbackAsync(string transactionId, string status, string gatewayResponse)
    {
        var payment = await _unitOfWork.Payments.GetByTransactionIdAsync(transactionId);
        if (payment == null) return false;
        
        payment.GatewayResponse = gatewayResponse;
        
        if (status.ToLower() == "success")
        {
            payment.Status = PaymentStatus.Completed;
            payment.CompletedAt = DateTime.UtcNow;
            
            // Update ticket
            var ticket = await _unitOfWork.Tickets.GetByIdAsync(payment.TicketId);
            if (ticket != null)
            {
                ticket.Status = TicketStatus.Booked;
                ticket.PaymentId = payment.Id;
                await _unitOfWork.Tickets.UpdateAsync(ticket);
            }
        }
        else
        {
            payment.Status = PaymentStatus.Failed;
            payment.FailureReason = "Payment gateway declined the transaction";
        }
        
        await _unitOfWork.Payments.UpdateAsync(payment);
        await _unitOfWork.SaveChangesAsync();
        
        return true;
    }
    
    public async Task<PaymentResponseDto?> GetPaymentByIdAsync(Guid id)
    {
        var payment = await _unitOfWork.Payments.GetByIdAsync(id);
        if (payment == null) return null;
        return MapToResponse(payment);
    }
    
    public async Task<IEnumerable<PaymentResponseDto>> GetPaymentsByUserAsync(Guid userId)
    {
        var payments = await _unitOfWork.Payments.GetByUserAsync(userId);
        return payments.Select(MapToResponse).ToList();
    }
    
    public async Task<IEnumerable<PaymentResponseDto>> GetPaymentsByTicketAsync(Guid ticketId)
    {
        var payments = await _unitOfWork.Payments.GetByTicketAsync(ticketId);
        return payments.Select(MapToResponse).ToList();
    }
    
    public async Task<PaymentResponseDto?> RefundPaymentAsync(Guid paymentId, RefundRequestDto request)
    {
        var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId);
        if (payment == null) return null;
        
        if (payment.Status != PaymentStatus.Completed)
        {
            throw new Exception("Can only refund completed payments");
        }
        
        if (request.Amount > payment.Amount)
        {
            throw new Exception("Refund amount cannot exceed original payment");
        }
        
        payment.Status = PaymentStatus.Refunded;
        payment.RefundAmount = request.Amount;
        payment.RefundedAt = DateTime.UtcNow;
        
        await _unitOfWork.Payments.UpdateAsync(payment);
        
        // Update ticket if full refund
        if (request.Amount >= payment.Amount)
        {
            var ticket = await _unitOfWork.Tickets.GetByIdAsync(payment.TicketId);
            if (ticket != null)
            {
                ticket.Status = TicketStatus.Cancelled;
                ticket.CancellationReason = $"Refunded: {request.Reason}";
                ticket.CancelledAt = DateTime.UtcNow;
                await _unitOfWork.Tickets.UpdateAsync(ticket);
                
                // Restore available seats
                await _unitOfWork.Schedules.IncrementAvailableSeatsAsync(ticket.ScheduleId);
            }
        }
        
        await _unitOfWork.SaveChangesAsync();
        
        return MapToResponse(payment);
    }
    
    public async Task<PaymentStatsDto> GetPaymentStatsAsync()
    {
        var allPayments = await _unitOfWork.Payments.GetAllAsync();
        var today = DateTime.UtcNow.Date;
        var todayEnd = today.AddDays(1);
        var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        
        var todayPayments = allPayments.Where(p => p.CreatedAt >= today && p.CreatedAt < todayEnd);
        var monthPayments = allPayments.Where(p => p.CreatedAt >= monthStart);
        
        return new PaymentStatsDto
        {
            TotalRevenue = allPayments.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount),
            TodayRevenue = todayPayments.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount),
            ThisMonthRevenue = monthPayments.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount),
            TotalPayments = allPayments.Count(),
            SuccessfulPayments = allPayments.Count(p => p.Status == PaymentStatus.Completed),
            FailedPayments = allPayments.Count(p => p.Status == PaymentStatus.Failed),
            RefundedPayments = allPayments.Count(p => p.Status == PaymentStatus.Refunded),
            TotalRefunded = allPayments.Where(p => p.RefundAmount.HasValue).Sum(p => p.RefundAmount.Value)
        };
    }
    
    public async Task<IEnumerable<PaymentResponseDto>> GetPaymentsByDateRangeAsync(DateTime from, DateTime to)
    {
        var payments = await _unitOfWork.Payments.GetPaymentsByDateRangeAsync(from, to);
        return payments.Select(MapToResponse).ToList();
    }
    
    private PaymentResponseDto MapToResponse(Payment payment)
    {
        return new PaymentResponseDto
        {
            Id = payment.Id,
            TicketId = payment.TicketId,
            BookingReference = payment.Ticket?.BookingReference ?? "",
            Amount = payment.Amount,
            Currency = payment.Currency,
            Status = payment.Status.ToString(),
            PaymentMethod = payment.PaymentMethod,
            TransactionId = payment.TransactionId,
            CreatedAt = payment.CreatedAt,
            CompletedAt = payment.CompletedAt
        };
    }
}
