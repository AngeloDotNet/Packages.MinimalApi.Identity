using MinimalApi.Identity.API.Entities;

namespace MinimalApi.Identity.API.Services.Interfaces;

public interface IEmailSavingService
{
    Task SaveEmailAsync(EmailSending email);
}