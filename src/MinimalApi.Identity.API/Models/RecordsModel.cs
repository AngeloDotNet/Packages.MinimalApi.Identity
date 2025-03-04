namespace MinimalApi.Identity.API.Models;

public record class RegisterModel(string FirstName, string LastName, string Username, string Email, string Password);
public record class LoginModel(string Username, string Password, bool RememberMe);
public record class AuthResponse(string AccessToken, string RefreshToken, DateTime ExpiredToken);
public record class ForgotPasswordModel(string Email);
public record class ResetPasswordModel(string Email, string Token, string Password);
public record class UserProfileModel(string Username, string Email, string FirstName, string LastName)
{
    public string FullName => $"{FirstName} {LastName}";
}