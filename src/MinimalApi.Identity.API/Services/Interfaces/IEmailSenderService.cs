using MinimalApi.Identity.API.Enums;

namespace MinimalApi.Identity.API.Services.Interfaces;

public interface IEmailSenderService
{
    Task SendEmailTypeAsync(string email, string callbackUrl, EmailSendingType typeSender);
    Task SendEmailAsync(string email, string subject, string message, EmailSendingType typeSender);
}