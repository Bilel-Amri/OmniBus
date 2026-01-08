using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace OmniBus.Infrastructure.Services;

/// <summary>
/// QR Code signing service to prevent ticket fraud
/// Generates encrypted/signed QR codes containing TicketId and UserId
/// </summary>
public interface IQrCodeService
{
    string GenerateSignedQrCode(int ticketId, int userId, DateTime validUntil);
    (bool isValid, int? ticketId, int? userId) ValidateSignedQrCode(string qrCode);
    byte[] GenerateQrCodeImage(string qrCode);
}

public class SecureQrCodeService : IQrCodeService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SecureQrCodeService> _logger;
    private readonly byte[] _encryptionKey;
    private readonly byte[] _signingKey;

    public SecureQrCodeService(IConfiguration configuration, ILogger<SecureQrCodeService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        
        // Get keys from configuration or generate (in production, use Azure Key Vault or similar)
        var keyString = _configuration["Security:QrCodeKey"] ?? "OmniBus-Secure-QR-Key-2026-Tunisia-Transport";
        _encryptionKey = SHA256.HashData(Encoding.UTF8.GetBytes(keyString));
        _signingKey = SHA256.HashData(Encoding.UTF8.GetBytes(keyString + "-signing"));
    }

    /// <summary>
    /// Generates a signed and encrypted QR code token
    /// Format: {EncryptedPayload}.{Signature}
    /// </summary>
    public string GenerateSignedQrCode(int ticketId, int userId, DateTime validUntil)
    {
        try
        {
            var payload = new QrCodePayload
            {
                TicketId = ticketId,
                UserId = userId,
                ValidUntil = validUntil,
                IssuedAt = DateTime.UtcNow,
                Nonce = Guid.NewGuid().ToString("N") // Prevent replay attacks
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var encryptedPayload = EncryptPayload(jsonPayload);
            var signature = SignPayload(encryptedPayload);

            return $"{encryptedPayload}.{signature}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate signed QR code for Ticket {TicketId}", ticketId);
            throw;
        }
    }

    /// <summary>
    /// Validates a signed QR code and extracts the payload
    /// </summary>
    public (bool isValid, int? ticketId, int? userId) ValidateSignedQrCode(string qrCode)
    {
        try
        {
            var parts = qrCode.Split('.');
            if (parts.Length != 2)
            {
                _logger.LogWarning("Invalid QR code format");
                return (false, null, null);
            }

            var encryptedPayload = parts[0];
            var providedSignature = parts[1];

            // Verify signature
            var expectedSignature = SignPayload(encryptedPayload);
            if (providedSignature != expectedSignature)
            {
                _logger.LogWarning("QR code signature mismatch - possible fraud attempt");
                return (false, null, null);
            }

            // Decrypt payload
            var jsonPayload = DecryptPayload(encryptedPayload);
            var payload = JsonSerializer.Deserialize<QrCodePayload>(jsonPayload);

            if (payload == null)
            {
                _logger.LogWarning("Failed to deserialize QR code payload");
                return (false, null, null);
            }

            // Check expiry
            if (DateTime.UtcNow > payload.ValidUntil)
            {
                _logger.LogWarning("QR code expired for Ticket {TicketId}", payload.TicketId);
                return (false, payload.TicketId, payload.UserId);
            }

            return (true, payload.TicketId, payload.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "QR code validation failed");
            return (false, null, null);
        }
    }

    /// <summary>
    /// Generates QR code image from the signed token
    /// </summary>
    public byte[] GenerateQrCodeImage(string qrCode)
    {
        using var qrGenerator = new QRCoder.QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(qrCode, QRCoder.QRCodeGenerator.ECCLevel.Q);
        var qrCodeImage = new QRCoder.PngByteQRCode(qrCodeData);
        return qrCodeImage.GetGraphic(20);
    }

    private string EncryptPayload(string payload)
    {
        using var aes = Aes.Create();
        aes.Key = _encryptionKey;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        var payloadBytes = Encoding.UTF8.GetBytes(payload);
        var encryptedBytes = encryptor.TransformFinalBlock(payloadBytes, 0, payloadBytes.Length);

        // Prepend IV to encrypted data
        var result = new byte[aes.IV.Length + encryptedBytes.Length];
        Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
        Buffer.BlockCopy(encryptedBytes, 0, result, aes.IV.Length, encryptedBytes.Length);

        return Convert.ToBase64String(result);
    }

    private string DecryptPayload(string encryptedPayload)
    {
        var encryptedBytes = Convert.FromBase64String(encryptedPayload);

        using var aes = Aes.Create();
        aes.Key = _encryptionKey;

        // Extract IV from the beginning
        var iv = new byte[aes.IV.Length];
        Buffer.BlockCopy(encryptedBytes, 0, iv, 0, iv.Length);
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        var cipherTextLength = encryptedBytes.Length - iv.Length;
        var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, iv.Length, cipherTextLength);

        return Encoding.UTF8.GetString(decryptedBytes);
    }

    private string SignPayload(string payload)
    {
        using var hmac = new HMACSHA256(_signingKey);
        var payloadBytes = Encoding.UTF8.GetBytes(payload);
        var hashBytes = hmac.ComputeHash(payloadBytes);
        return Convert.ToBase64String(hashBytes);
    }

    private class QrCodePayload
    {
        public int TicketId { get; set; }
        public int UserId { get; set; }
        public DateTime ValidUntil { get; set; }
        public DateTime IssuedAt { get; set; }
        public string Nonce { get; set; } = string.Empty;
    }
}
