using MinimalApi.Identity.API.Models;

namespace MinimalApi.Identity.API.Services.Interfaces;

public interface IAccountService
{
    Task<string> ConfirmEmailAsync(string userId, string token);
    Task<string> ChangeEmailAsync(ChangeEmailModel inputModel);
    Task<string> ConfirmEmailChangeAsync(string userId, string email, string token);
}
