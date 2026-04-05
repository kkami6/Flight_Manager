using FluentEmail.Core;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

public class EmailService : IEmailService, IEmailSender
{
    private readonly IFluentEmail _fluentEmail;

    public EmailService(IFluentEmail fluentEmail)
    {
        _fluentEmail = fluentEmail;
    }

    public async Task SendReservationConfirmationAsync(string userEmail, int flightNumber, int reservationId)
    {
        await _fluentEmail
            .To(userEmail)
            .Subject("Flight Reservation Confirmed!")
            .Body($"Thank you for booking flight {flightNumber}. Your Reference ID is: {reservationId}")
            .SendAsync();
    }

    // 2. ADD THIS: The method Identity is looking for
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        await _fluentEmail
            .To(email)
            .Subject(subject)
            .Body(htmlMessage, isHtml: true)
            .SendAsync();
    }
}