namespace MinimalApi.Identity.API.Models;

public record class AuthResponse(string AccessToken, string RefreshToken, DateTime ExpiredToken);