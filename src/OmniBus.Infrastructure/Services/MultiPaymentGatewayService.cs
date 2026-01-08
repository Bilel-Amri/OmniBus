using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OmniBus.Application.Interfaces;

namespace OmniBus.Infrastructure.Services;

/// <summary>
/// Multi-gateway payment service supporting D17, Flouci, and Konnect
/// </summary>
public interface IMultiPaymentGateway
{
    Task<MultiGatewayPaymentResult> InitiatePaymentAsync(decimal amount, string currency, string orderId, PaymentGateway gateway, Dictionary<string, string>? metadata = null);
    Task<MultiGatewayVerificationResult> VerifyPaymentAsync(string transactionId, PaymentGateway gateway);
    Task<bool> RefundPaymentAsync(string transactionId, decimal amount, PaymentGateway gateway, string reason);
}

public enum PaymentGateway
{
    D17 = 0,
    Flouci = 1,
    Konnect = 2
}

public class MultiGatewayPaymentResult
{
    public bool Success { get; set; }
    public string? PaymentUrl { get; set; }
    public string? TransactionId { get; set; }
    public string? ErrorMessage { get; set; }
    public PaymentGateway Gateway { get; set; }
}

public class MultiGatewayVerificationResult
{
    public bool Success { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? TransactionId { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? PaymentDate { get; set; }
}

public class MultiPaymentGatewayService : IMultiPaymentGateway
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<MultiPaymentGatewayService> _logger;

    public MultiPaymentGatewayService(HttpClient httpClient, IConfiguration configuration, ILogger<MultiPaymentGatewayService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<MultiGatewayPaymentResult> InitiatePaymentAsync(decimal amount, string currency, string orderId, PaymentGateway gateway, Dictionary<string, string>? metadata = null)
    {
        return gateway switch
        {
            PaymentGateway.D17 => await InitiateD17PaymentAsync(amount, currency, orderId, metadata),
            PaymentGateway.Flouci => await InitiateFlouciPaymentAsync(amount, currency, orderId, metadata),
            PaymentGateway.Konnect => await InitiateKonnectPaymentAsync(amount, currency, orderId, metadata),
            _ => throw new ArgumentException($"Unsupported gateway: {gateway}")
        };
    }

    public async Task<MultiGatewayVerificationResult> VerifyPaymentAsync(string transactionId, PaymentGateway gateway)
    {
        return gateway switch
        {
            PaymentGateway.D17 => await VerifyD17PaymentAsync(transactionId),
            PaymentGateway.Flouci => await VerifyFlouciPaymentAsync(transactionId),
            PaymentGateway.Konnect => await VerifyKonnectPaymentAsync(transactionId),
            _ => throw new ArgumentException($"Unsupported gateway: {gateway}")
        };
    }

    public async Task<bool> RefundPaymentAsync(string transactionId, decimal amount, PaymentGateway gateway, string reason)
    {
        return gateway switch
        {
            PaymentGateway.D17 => await RefundD17PaymentAsync(transactionId, amount, reason),
            PaymentGateway.Flouci => await RefundFlouciPaymentAsync(transactionId, amount, reason),
            PaymentGateway.Konnect => await RefundKonnectPaymentAsync(transactionId, amount, reason),
            _ => throw new ArgumentException($"Unsupported gateway: {gateway}")
        };
    }

    #region D17 Payment Gateway

    private async Task<MultiGatewayPaymentResult> InitiateD17PaymentAsync(decimal amount, string currency, string orderId, Dictionary<string, string>? metadata)
    {
        try
        {
            var apiKey = _configuration["D17Payment:ApiKey"];
            var baseUrl = _configuration["D17Payment:BaseUrl"];
            var merchantId = _configuration["D17Payment:MerchantId"];
            var callbackUrl = _configuration["AppBaseUrl"] + "/api/payments/callback/d17";

            var request = new
            {
                merchant_id = merchantId,
                amount = amount * 1000, // D17 uses millimes
                currency = currency,
                order_id = orderId,
                callback_url = callbackUrl,
                metadata = metadata
            };

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/payments/init", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<D17InitResponse>();
                return new MultiGatewayPaymentResult
                {
                    Success = true,
                    PaymentUrl = result?.PaymentUrl,
                    TransactionId = result?.TransactionId,
                    Gateway = PaymentGateway.D17
                };
            }

            return new MultiGatewayPaymentResult
            {
                Success = false,
                ErrorMessage = await response.Content.ReadAsStringAsync(),
                Gateway = PaymentGateway.D17
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "D17 payment initiation failed");
            return new MultiGatewayPaymentResult { Success = false, ErrorMessage = ex.Message, Gateway = PaymentGateway.D17 };
        }
    }

    private async Task<MultiGatewayVerificationResult> VerifyD17PaymentAsync(string transactionId)
    {
        try
        {
            var apiKey = _configuration["D17Payment:ApiKey"];
            var baseUrl = _configuration["D17Payment:BaseUrl"];

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            var response = await _httpClient.GetAsync($"{baseUrl}/payments/{transactionId}/status");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<D17StatusResponse>();
                return new MultiGatewayVerificationResult
                {
                    Success = result?.Status == "completed",
                    Status = result?.Status ?? "unknown",
                    Amount = (result?.Amount ?? 0) / 1000m,
                    TransactionId = transactionId,
                    PaymentDate = result?.PaymentDate
                };
            }

            return new MultiGatewayVerificationResult { Success = false, Status = "failed", ErrorMessage = "Verification failed" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "D17 verification failed");
            return new MultiGatewayVerificationResult { Success = false, Status = "error", ErrorMessage = ex.Message };
        }
    }

    private async Task<bool> RefundD17PaymentAsync(string transactionId, decimal amount, string reason)
    {
        try
        {
            var apiKey = _configuration["D17Payment:ApiKey"];
            var baseUrl = _configuration["D17Payment:BaseUrl"];

            var request = new { amount = amount * 1000, reason };
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/payments/{transactionId}/refund", request);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "D17 refund failed");
            return false;
        }
    }

    #endregion

    #region Flouci Payment Gateway

    private async Task<MultiGatewayPaymentResult> InitiateFlouciPaymentAsync(decimal amount, string currency, string orderId, Dictionary<string, string>? metadata)
    {
        try
        {
            var apiKey = _configuration["Flouci:ApiKey"];
            var appToken = _configuration["Flouci:AppToken"];
            var baseUrl = _configuration["Flouci:BaseUrl"] ?? "https://developers.flouci.com";

            var request = new
            {
                app_token = appToken,
                app_secret = apiKey,
                amount = amount,
                accept_card = true,
                session_timeout_secs = 1200,
                success_link = _configuration["AppBaseUrl"] + "/payment/success",
                fail_link = _configuration["AppBaseUrl"] + "/payment/failed",
                developer_tracking_id = orderId
            };

            var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/api/generate_payment", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<FlouciInitResponse>();
                return new MultiGatewayPaymentResult
                {
                    Success = true,
                    PaymentUrl = $"{baseUrl}/checkout/{result?.PaymentId}",
                    TransactionId = result?.PaymentId,
                    Gateway = PaymentGateway.Flouci
                };
            }

            return new MultiGatewayPaymentResult { Success = false, ErrorMessage = "Flouci init failed", Gateway = PaymentGateway.Flouci };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Flouci payment initiation failed");
            return new MultiGatewayPaymentResult { Success = false, ErrorMessage = ex.Message, Gateway = PaymentGateway.Flouci };
        }
    }

    private async Task<MultiGatewayVerificationResult> VerifyFlouciPaymentAsync(string transactionId)
    {
        try
        {
            var apiKey = _configuration["Flouci:ApiKey"];
            var appToken = _configuration["Flouci:AppToken"];
            var baseUrl = _configuration["Flouci:BaseUrl"] ?? "https://developers.flouci.com";

            var request = new { id = transactionId };
            var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/api/verify_payment", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<FlouciVerifyResponse>();
                return new MultiGatewayVerificationResult
                {
                    Success = result?.Status == "SUCCESS",
                    Status = result?.Status ?? "UNKNOWN",
                    Amount = result?.Amount ?? 0,
                    TransactionId = transactionId
                };
            }

            return new MultiGatewayVerificationResult { Success = false, Status = "failed" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Flouci verification failed");
            return new MultiGatewayVerificationResult { Success = false, Status = "error", ErrorMessage = ex.Message };
        }
    }

    private async Task<bool> RefundFlouciPaymentAsync(string transactionId, decimal amount, string reason)
    {
        // Flouci refunds typically require manual processing or support ticket
        _logger.LogWarning("Flouci refund requested for {TransactionId} - manual processing required", transactionId);
        return false;
    }

    #endregion

    #region Konnect Payment Gateway

    private async Task<MultiGatewayPaymentResult> InitiateKonnectPaymentAsync(decimal amount, string currency, string orderId, Dictionary<string, string>? metadata)
    {
        try
        {
            var apiKey = _configuration["Konnect:ApiKey"];
            var receiverId = _configuration["Konnect:ReceiverId"];
            var baseUrl = _configuration["Konnect:BaseUrl"] ?? "https://api.konnect.network";

            var request = new
            {
                receiverWalletId = receiverId,
                amount = ((int)(amount * 1000)).ToString(), // Millimes as string
                token = "TND",
                description = $"OmniBus Ticket - {orderId}",
                acceptedPaymentMethods = new[] { "wallet", "bank_card", "e-DINAR" },
                lifespan = 10,
                checkoutForm = true,
                addPaymentFeesToAmount = true,
                firstName = metadata?.GetValueOrDefault("firstName", ""),
                lastName = metadata?.GetValueOrDefault("lastName", ""),
                phoneNumber = metadata?.GetValueOrDefault("phone", ""),
                email = metadata?.GetValueOrDefault("email", ""),
                orderId = orderId,
                webhook = _configuration["AppBaseUrl"] + "/api/payments/webhook/konnect",
                successUrl = _configuration["AppBaseUrl"] + "/payment/success",
                failUrl = _configuration["AppBaseUrl"] + "/payment/failed"
            };

            _httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);
            var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/api/v2/payments/init-payment", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<KonnectInitResponse>();
                return new MultiGatewayPaymentResult
                {
                    Success = true,
                    PaymentUrl = result?.PayUrl,
                    TransactionId = result?.PaymentRef,
                    Gateway = PaymentGateway.Konnect
                };
            }

            return new MultiGatewayPaymentResult { Success = false, ErrorMessage = "Konnect init failed", Gateway = PaymentGateway.Konnect };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Konnect payment initiation failed");
            return new MultiGatewayPaymentResult { Success = false, ErrorMessage = ex.Message, Gateway = PaymentGateway.Konnect };
        }
    }

    private async Task<MultiGatewayVerificationResult> VerifyKonnectPaymentAsync(string transactionId)
    {
        try
        {
            var apiKey = _configuration["Konnect:ApiKey"];
            var baseUrl = _configuration["Konnect:BaseUrl"] ?? "https://api.konnect.network";

            _httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);
            var response = await _httpClient.GetAsync($"{baseUrl}/api/v2/payments/{transactionId}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<KonnectStatusResponse>();
                return new MultiGatewayVerificationResult
                {
                    Success = result?.Status == "completed",
                    Status = result?.Status ?? "unknown",
                    Amount = decimal.Parse(result?.Amount ?? "0") / 1000m,
                    TransactionId = transactionId,
                    PaymentDate = result?.PaymentDate
                };
            }

            return new MultiGatewayVerificationResult { Success = false, Status = "failed" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Konnect verification failed");
            return new MultiGatewayVerificationResult { Success = false, Status = "error", ErrorMessage = ex.Message };
        }
    }

    private async Task<bool> RefundKonnectPaymentAsync(string transactionId, decimal amount, string reason)
    {
        // Konnect refunds require manual processing through their dashboard
        _logger.LogWarning("Konnect refund requested for {TransactionId} - manual processing required", transactionId);
        return false;
    }

    #endregion

    #region Response DTOs

    private class D17InitResponse
    {
        public string? PaymentUrl { get; set; }
        public string? TransactionId { get; set; }
    }

    private class D17StatusResponse
    {
        public string? Status { get; set; }
        public decimal Amount { get; set; }
        public DateTime? PaymentDate { get; set; }
    }

    private class FlouciInitResponse
    {
        public string? PaymentId { get; set; }
    }

    private class FlouciVerifyResponse
    {
        public string? Status { get; set; }
        public decimal Amount { get; set; }
    }

    private class KonnectInitResponse
    {
        public string? PayUrl { get; set; }
        public string? PaymentRef { get; set; }
    }

    private class KonnectStatusResponse
    {
        public string? Status { get; set; }
        public string? Amount { get; set; }
        public DateTime? PaymentDate { get; set; }
    }

    #endregion
}
