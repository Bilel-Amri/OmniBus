namespace OmniBus.Application.Interfaces;

/// <summary>
/// Email service interface
/// </summary>
public interface IEmailService
{
    Task SendBookingConfirmationAsync(string toEmail, string passengerName, string bookingReference, string routeInfo, DateTime departureTime, decimal price);
    Task SendPaymentConfirmationAsync(string toEmail, string passengerName, string bookingReference, decimal amount, string transactionId);
    Task SendTripReminderAsync(string toEmail, string passengerName, string bookingReference, string routeInfo, DateTime departureTime);
    Task SendCancellationNotificationAsync(string toEmail, string passengerName, string bookingReference, string routeInfo);
}
