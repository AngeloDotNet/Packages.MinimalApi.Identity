namespace MinimalApi.Identity.API.Models;

public class ResetPasswordModel
{
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;
    public string Password { get; set; } = null!;
}