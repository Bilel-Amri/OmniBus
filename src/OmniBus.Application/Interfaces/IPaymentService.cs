using OmniBus.Application.DTOs;

namespace OmniBus.Application.Interfaces;

public interface IPaymentService
{
    Task<PaymentResultDto> ProcessPaymentAsync(Guid userId, CreatePaymentDto request);
    Task<bool> HandlePaymentCallbackAsync(string transactionId, string status, string gatewayResponse);
    Task<PaymentResponseDto?> GetPaymentByIdAsync(Guid id);
    Task<IEnumerable<PaymentResponseDto>> GetPaymentsByUserAsync(Guid userId);
    Task<IEnumerable<PaymentResponseDto>> GetPaymentsByTicketAsync(Guid ticketId);
    Task<PaymentResponseDto?> RefundPaymentAsync(Guid paymentId, RefundRequestDto request);
    Task<PaymentStatsDto> GetPaymentStatsAsync();
    Task<IEnumerable<PaymentResponseDto>> GetPaymentsByDateRangeAsync(DateTime from, DateTime to);
}
