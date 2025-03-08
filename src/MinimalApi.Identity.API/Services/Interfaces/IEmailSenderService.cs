namespace MinimalApi.Identity.API.Services.Interfaces;

public interface IEmailSenderService
{
    Task SendEmailTypeAsync(string email, string callbackUrl, int typeSender);
}