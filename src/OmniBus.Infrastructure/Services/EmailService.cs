using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using OmniBus.Application.Interfaces;

namespace OmniBus.Infrastructure.Services;

/// <summary>
/// Email service implementation using SMTP
/// </summary>
public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _smtpUsername;
    private readonly string _smtpPassword;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
        _smtpHost = configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
        _smtpPort = int.Parse(configuration["Email:SmtpPort"] ?? "587");
        _smtpUsername = configuration["Email:SmtpUsername"] ?? "";
        _smtpPassword = configuration["Email:SmtpPassword"] ?? "";
        _fromEmail = configuration["Email:FromEmail"] ?? "noreply@omnibus.tn";
        _fromName = configuration["Email:FromName"] ?? "OmniBus Tunisia";
    }

    public async Task SendBookingConfirmationAsync(string toEmail, string passengerName, string bookingReference, string routeInfo, DateTime departureTime, decimal price)
    {
        var subject = $"Booking Confirmation - {bookingReference}";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2 style='color: #0F766E;'>Booking Confirmed!</h2>
                <p>Dear {passengerName},</p>
                <p>Your bus ticket has been successfully booked.</p>
                <div style='background-color: #f5f5f5; padding: 20px; border-radius: 5px; margin: 20px 0;'>
                    <p><strong>Booking Reference:</strong> {bookingReference}</p>
                    <p><strong>Route:</strong> {routeInfo}</p>
                    <p><strong>Departure:</strong> {departureTime:dddd, MMMM dd, yyyy 'at' HH:mm}</p>
                    <p><strong>Total Amount:</strong> {price:F2} TND</p>
                </div>
                <p>Please arrive at the station at least 15 minutes before departure.</p>
                <p>Show your QR code to the driver when boarding.</p>
                <br/>
                <p>Safe travels!</p>
                <p><strong>OmniBus Team</strong></p>
            </body>
            </html>
        ";

        await SendEmailAsync(toEmail, subject, body);
    }

    public async Task SendPaymentConfirmationAsync(string toEmail, string passengerName, string bookingReference, decimal amount, string transactionId)
    {
        var subject = $"Payment Received - {bookingReference}";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2 style='color: #0F766E;'>Payment Confirmed!</h2>
                <p>Dear {passengerName},</p>
                <p>We have received your payment for booking {bookingReference}.</p>
                <div style='background-color: #f5f5f5; padding: 20px; border-radius: 5px; margin: 20px 0;'>
                    <p><strong>Amount Paid:</strong> {amount:F2} TND</p>
                    <p><strong>Transaction ID:</strong> {transactionId}</p>
                    <p><strong>Payment Date:</strong> {DateTime.UtcNow:dddd, MMMM dd, yyyy 'at' HH:mm}</p>
                </div>
                <p>Your ticket is now confirmed and ready to use.</p>
                <br/>
                <p>Thank you for choosing OmniBus!</p>
                <p><strong>OmniBus Team</strong></p>
            </body>
            </html>
        ";

        await SendEmailAsync(toEmail, subject, body);
    }

    public async Task SendTripReminderAsync(string toEmail, string passengerName, string bookingReference, string routeInfo, DateTime departureTime)
    {
        var subject = $"Trip Reminder - Departure Tomorrow";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2 style='color: #0F766E;'>Trip Reminder</h2>
                <p>Dear {passengerName},</p>
                <p>This is a friendly reminder about your upcoming trip.</p>
                <div style='background-color: #fff3cd; padding: 20px; border-radius: 5px; margin: 20px 0; border-left: 4px solid #ffc107;'>
                    <p><strong>Booking Reference:</strong> {bookingReference}</p>
                    <p><strong>Route:</strong> {routeInfo}</p>
                    <p><strong>Departure:</strong> {departureTime:dddd, MMMM dd, yyyy 'at' HH:mm}</p>
                </div>
                <p><strong>Important reminders:</strong></p>
                <ul>
                    <li>Arrive at the station at least 15 minutes early</li>
                    <li>Have your QR code ready</li>
                    <li>Check weather conditions before traveling</li>
                </ul>
                <br/>
                <p>Have a safe journey!</p>
                <p><strong>OmniBus Team</strong></p>
            </body>
            </html>
        ";

        await SendEmailAsync(toEmail, subject, body);
    }

    public async Task SendCancellationNotificationAsync(string toEmail, string passengerName, string bookingReference, string routeInfo)
    {
        var subject = $"Booking Cancelled - {bookingReference}";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2 style='color: #dc2626;'>Booking Cancelled</h2>
                <p>Dear {passengerName},</p>
                <p>Your booking has been cancelled as requested.</p>
                <div style='background-color: #fee2e2; padding: 20px; border-radius: 5px; margin: 20px 0;'>
                    <p><strong>Booking Reference:</strong> {bookingReference}</p>
                    <p><strong>Route:</strong> {routeInfo}</p>
                    <p><strong>Cancellation Date:</strong> {DateTime.UtcNow:dddd, MMMM dd, yyyy 'at' HH:mm}</p>
                </div>
                <p>Your refund will be processed within 3-5 business days.</p>
                <p>We hope to serve you again soon!</p>
                <br/>
                <p><strong>OmniBus Team</strong></p>
            </body>
            </html>
        ";

        await SendEmailAsync(toEmail, subject, body);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        try
        {
            using var client = new SmtpClient(_smtpHost, _smtpPort)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_smtpUsername, _smtpPassword)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail, _fromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            // Log error but don't throw - email failures shouldn't break the booking flow
            Console.WriteLine($"Failed to send email to {toEmail}: {ex.Message}");
        }
    }
}
