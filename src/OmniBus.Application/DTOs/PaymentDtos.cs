namespace OmniBus.Application.DTOs;

public class PaymentDto : BaseDto
{
    public Guid TicketId { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "TND";
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public DateTime? CompletedAt { get; set; }
}

public class PaymentResponseDto : BaseDto
{
    public Guid TicketId { get; set; }
    public Guid UserId { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "TND";
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public DateTime? CompletedAt { get; set; }
}

public class CreatePaymentDto
{
    public Guid TicketId { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? CardNumber { get; set; }
    public string? CardExpiry { get; set; }
    public string? CardCvc { get; set; }
}

public class PaymentResultDto
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? TransactionId { get; set; }
    public string? PaymentUrl { get; set; }
    public PaymentResponseDto? Payment { get; set; }
}

public class RefundRequestDto
{
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class PaymentStatsDto
{
    public decimal TotalRevenue { get; set; }
    public decimal TodayRevenue { get; set; }
    public decimal ThisMonthRevenue { get; set; }
    public int TotalPayments { get; set; }
    public int SuccessfulPayments { get; set; }
    public int FailedPayments { get; set; }
    public int RefundedPayments { get; set; }
    public decimal TotalRefunded { get; set; }
}
