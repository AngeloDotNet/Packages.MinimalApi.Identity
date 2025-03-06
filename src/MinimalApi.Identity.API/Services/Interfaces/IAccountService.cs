using Microsoft.AspNetCore.Http.HttpResults;
using MinimalApi.Identity.API.Models;

namespace MinimalApi.Identity.API.Services.Interfaces;

public interface IAccountService
{
    Task<Results<Ok<string>, BadRequest<string>>> ConfirmEmailAsync(string userId, string token);
    Task<Results<Ok<string>, BadRequest<string>>> ChangeEmailAsync(ChangeEmailModel inputModel);
    Task<Results<Ok<string>, BadRequest<string>>> ConfirmEmailChangeAsync(string userId, string email, string token);
}
