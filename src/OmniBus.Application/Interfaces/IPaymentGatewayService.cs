namespace OmniBus.Application.Interfaces;

/// <summary>
/// Payment gateway service interface (D17 Tunisia)
/// </summary>
public interface IPaymentGatewayService
{
    Task<PaymentInitiationResult> InitiatePaymentAsync(decimal amount, string currency, string orderId, string customerEmail);
    Task<PaymentVerificationResult> VerifyPaymentAsync(string transactionId);
    Task<RefundResult> ProcessRefundAsync(string transactionId, decimal amount);
}

public class PaymentInitiationResult
{
    public bool Success { get; set; }
    public string? PaymentUrl { get; set; }
    public string? TransactionId { get; set; }
    public string? ErrorMessage { get; set; }
}

public class PaymentVerificationResult
{
    public bool Success { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? TransactionId { get; set; }
    public decimal Amount { get; set; }
    public string? ErrorMessage { get; set; }
}

public class RefundResult
{
    public bool Success { get; set; }
    public string? RefundId { get; set; }
    public string? ErrorMessage { get; set; }
}
