namespace MinimalApi.Identity.API.Services.Interfaces;

public interface IEmailSenderService
{
    Task SendEmailAsync(string email, string subject, string message, int emailSendingType);
}