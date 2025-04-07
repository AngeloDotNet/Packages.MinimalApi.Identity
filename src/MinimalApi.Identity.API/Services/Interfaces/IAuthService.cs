﻿using MinimalApi.Identity.API.Models;

namespace MinimalApi.Identity.API.Services.Interfaces;

public interface IAuthService
{
    Task<string> RegisterAsync(RegisterModel model);
    Task<AuthResponseModel> LoginAsync(LoginModel model);
    Task<string> LogoutAsync();
}