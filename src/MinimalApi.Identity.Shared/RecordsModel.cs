namespace MinimalApi.Identity.Shared;

public record class AuthResponse(string AccessToken, string RefreshToken, DateTime ExpiredToken);