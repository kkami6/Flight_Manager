using FluentEmail.Core;

public class EmailService : IEmailService
{
    private readonly IFluentEmail _fluentEmail;

    public EmailService(IFluentEmail fluentEmail)
    {
        _fluentEmail = fluentEmail;
    }

    public async Task SendReservationConfirmationAsync(string userEmail, string flightNumber, string reservationId)
    {
        var email = _fluentEmail
            .To(userEmail)
            .Subject("Flight Reservation Confirmed!")
            .Body($"Thank you for booking flight {flightNumber}. Your Reference ID is: {reservationId}");

        await email.SendAsync();
    }
}