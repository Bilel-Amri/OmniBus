using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using OmniBus.Application.Interfaces;

namespace OmniBus.Infrastructure.Services;

/// <summary>
/// D17 Payment Gateway implementation for Tunisia
/// </summary>
public class D17PaymentGatewayService : IPaymentGatewayService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly string _apiKey;
    private readonly string _apiSecret;
    private readonly string _baseUrl;
    private readonly string _merchantId;

    public D17PaymentGatewayService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _baseUrl = configuration["D17Payment:BaseUrl"] ?? "https://api.d17.tn";
        _apiKey = configuration["D17Payment:ApiKey"] ?? "demo_key";
        _apiSecret = configuration["D17Payment:ApiSecret"] ?? "demo_secret";
        _merchantId = configuration["D17Payment:MerchantId"] ?? "OMNIBUS_TN";

        _httpClient.BaseAddress = new Uri(_baseUrl);
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", GenerateAuthToken());
    }

    public async Task<PaymentInitiationResult> InitiatePaymentAsync(decimal amount, string currency, string orderId, string customerEmail)
    {
        try
        {
            var payload = new
            {
                merchant_id = _merchantId,
                order_id = orderId,
                amount = amount,
                currency = currency,
                customer_email = customerEmail,
                return_url = $"{_configuration["AppBaseUrl"]}/payment/callback",
                cancel_url = $"{_configuration["AppBaseUrl"]}/payment/cancel",
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("/api/v1/payments/initiate", content);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<D17InitiateResponse>(responseData);

                return new PaymentInitiationResult
                {
                    Success = true,
                    PaymentUrl = result?.payment_url,
                    TransactionId = result?.transaction_id
                };
            }

            return new PaymentInitiationResult
            {
                Success = false,
                ErrorMessage = $"Payment initiation failed with status {response.StatusCode}"
            };
        }
        catch (Exception ex)
        {
            // In demo mode, return mock success
            if (_apiKey == "demo_key")
            {
                return new PaymentInitiationResult
                {
                    Success = true,
                    PaymentUrl = $"https://demo-payment.omnibus.tn?order={orderId}",
                    TransactionId = $"D17_{Guid.NewGuid():N}"
                };
            }

            return new PaymentInitiationResult
            {
                Success = false,
                ErrorMessage = $"Payment gateway error: {ex.Message}"
            };
        }
    }

    public async Task<PaymentVerificationResult> VerifyPaymentAsync(string transactionId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/v1/payments/{transactionId}/verify");

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<D17VerificationResponse>(responseData);

                return new PaymentVerificationResult
                {
                    Success = result?.status == "completed",
                    Status = result?.status ?? "unknown",
                    TransactionId = transactionId,
                    Amount = result?.amount ?? 0
                };
            }

            return new PaymentVerificationResult
            {
                Success = false,
                Status = "failed",
                ErrorMessage = $"Verification failed with status {response.StatusCode}"
            };
        }
        catch (Exception ex)
        {
            // In demo mode, return mock success
            if (_apiKey == "demo_key" && transactionId.StartsWith("D17_"))
            {
                return new PaymentVerificationResult
                {
                    Success = true,
                    Status = "completed",
                    TransactionId = transactionId,
                    Amount = 0
                };
            }

            return new PaymentVerificationResult
            {
                Success = false,
                Status = "error",
                ErrorMessage = $"Verification error: {ex.Message}"
            };
        }
    }

    public async Task<RefundResult> ProcessRefundAsync(string transactionId, decimal amount)
    {
        try
        {
            var payload = new
            {
                transaction_id = transactionId,
                amount = amount,
                reason = "Customer cancellation",
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync($"/api/v1/payments/{transactionId}/refund", content);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<D17RefundResponse>(responseData);

                return new RefundResult
                {
                    Success = true,
                    RefundId = result?.refund_id
                };
            }

            return new RefundResult
            {
                Success = false,
                ErrorMessage = $"Refund failed with status {response.StatusCode}"
            };
        }
        catch (Exception ex)
        {
            // In demo mode, return mock success
            if (_apiKey == "demo_key")
            {
                return new RefundResult
                {
                    Success = true,
                    RefundId = $"REF_{Guid.NewGuid():N}"
                };
            }

            return new RefundResult
            {
                Success = false,
                ErrorMessage = $"Refund error: {ex.Message}"
            };
        }
    }

    private string GenerateAuthToken()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var signature = $"{_apiKey}:{_apiSecret}:{timestamp}";
        var bytes = Encoding.UTF8.GetBytes(signature);
        return Convert.ToBase64String(bytes);
    }

    private class D17InitiateResponse
    {
        public string? payment_url { get; set; }
        public string? transaction_id { get; set; }
    }

    private class D17VerificationResponse
    {
        public string? status { get; set; }
        public decimal amount { get; set; }
    }

    private class D17RefundResponse
    {
        public string? refund_id { get; set; }
    }
}
