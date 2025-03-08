using Microsoft.AspNetCore.Http;
using MinimalApi.Identity.API.Models;

namespace MinimalApi.Identity.API.Services.Interfaces;

public interface IAuthService
{
    Task<IResult> RegisterAsync(RegisterModel model);
    Task<IResult> LoginAsync(LoginModel model);
    Task<IResult> LogoutAsync();
}