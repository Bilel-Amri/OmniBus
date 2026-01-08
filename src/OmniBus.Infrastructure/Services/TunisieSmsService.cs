using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace OmniBus.Infrastructure.Services;

/// <summary>
/// SMS Gateway service for Tunisian operators (Tunisie Telecom, Ooredoo, Orange)
/// Sends booking confirmations, cancellations, and alerts
/// </summary>
public interface ISmsService
{
    Task<bool> SendBookingConfirmationAsync(string phoneNumber, string bookingReference, DateTime departureTime, string routeName);
    Task<bool> SendCancellationNoticeAsync(string phoneNumber, string bookingReference, string reason);
    Task<bool> SendBulkSmsAsync(List<string> phoneNumbers, string message);
    Task<bool> SendOtpAsync(string phoneNumber, string otpCode);
}

public class TunisieSmsService : ISmsService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TunisieSmsService> _logger;

    public TunisieSmsService(HttpClient httpClient, IConfiguration configuration, ILogger<TunisieSmsService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Sends booking confirmation SMS with ticket details
    /// </summary>
    public async Task<bool> SendBookingConfirmationAsync(string phoneNumber, string bookingReference, DateTime departureTime, string routeName)
    {
        var message = $"OmniBus: Réservation confirmée!\n" +
                      $"Ref: {bookingReference}\n" +
                      $"Départ: {departureTime:dd/MM/yyyy HH:mm}\n" +
                      $"Ligne: {routeName}\n" +
                      $"Bon voyage!";

        return await SendSmsAsync(phoneNumber, message);
    }

    /// <summary>
    /// Sends trip cancellation notice with reason
    /// </summary>
    public async Task<bool> SendCancellationNoticeAsync(string phoneNumber, string bookingReference, string reason)
    {
        var message = $"OmniBus: ANNULATION\n" +
                      $"Ref: {bookingReference}\n" +
                      $"Motif: {reason}\n" +
                      $"Remboursement en cours.\n" +
                      $"Pour info: support@omnibus.tn";

        return await SendSmsAsync(phoneNumber, message);
    }

    /// <summary>
    /// Sends SMS to multiple recipients (for bulk cancellations)
    /// </summary>
    public async Task<bool> SendBulkSmsAsync(List<string> phoneNumbers, string message)
    {
        var tasks = phoneNumbers.Select(phone => SendSmsAsync(phone, message));
        var results = await Task.WhenAll(tasks);
        
        var successCount = results.Count(r => r);
        _logger.LogInformation("Bulk SMS sent: {Success}/{Total}", successCount, phoneNumbers.Count);
        
        return successCount == phoneNumbers.Count;
    }

    /// <summary>
    /// Sends OTP code for authentication
    /// </summary>
    public async Task<bool> SendOtpAsync(string phoneNumber, string otpCode)
    {
        var message = $"OmniBus: Votre code de vérification est: {otpCode}\n" +
                      $"Ne partagez ce code avec personne.";

        return await SendSmsAsync(phoneNumber, message);
    }

    private async Task<bool> SendSmsAsync(string phoneNumber, string message)
    {
        try
        {
            var apiKey = _configuration["TunisieSms:ApiKey"];
            var senderId = _configuration["TunisieSms:SenderId"] ?? "OmniBus";
            var apiUrl = _configuration["TunisieSms:ApiUrl"] ?? "https://api.tunisiesms.tn/v2/send";

            // Clean phone number (remove spaces, ensure +216 prefix)
            var cleanPhone = CleanPhoneNumber(phoneNumber);

            var request = new
            {
                api_key = apiKey,
                sender_id = senderId,
                recipients = new[] { cleanPhone },
                message = message,
                type = "text"
            };

            var response = await _httpClient.PostAsJsonAsync(apiUrl, request);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("SMS sent successfully to {Phone}", MaskPhoneNumber(cleanPhone));
                return true;
            }
            
            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("SMS sending failed: {Error}", error);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SMS sending exception for {Phone}", MaskPhoneNumber(phoneNumber));
            return false;
        }
    }

    private static string CleanPhoneNumber(string phoneNumber)
    {
        // Remove all non-digit characters
        var digits = new string(phoneNumber.Where(char.IsDigit).ToArray());
        
        // Ensure Tunisia country code (+216)
        if (digits.StartsWith("216"))
        {
            return "+" + digits;
        }
        else if (digits.Length == 8)
        {
            return "+216" + digits;
        }
        else if (digits.StartsWith("00216"))
        {
            return "+" + digits.Substring(2);
        }
        
        return "+" + digits;
    }

    private static string MaskPhoneNumber(string phone)
    {
        if (phone.Length < 8) return "***";
        return phone.Substring(0, phone.Length - 4) + "****";
    }
}
