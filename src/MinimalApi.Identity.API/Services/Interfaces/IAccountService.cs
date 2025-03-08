using Microsoft.AspNetCore.Http;
using MinimalApi.Identity.API.Models;

namespace MinimalApi.Identity.API.Services.Interfaces;

public interface IAccountService
{
    Task<IResult> ConfirmEmailAsync(string userId, string token);
    Task<IResult> ChangeEmailAsync(ChangeEmailModel inputModel);
    Task<IResult> ConfirmEmailChangeAsync(string userId, string email, string token);
}
