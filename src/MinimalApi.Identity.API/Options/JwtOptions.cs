using System.ComponentModel.DataAnnotations;

namespace MinimalApi.Identity.API.Options;

public class JwtOptions
{
    [Required, MinLength(1, ErrorMessage = "Issuer cannot be empty.")]
    public string Issuer { get; init; } = null!;

    [Required, MinLength(1, ErrorMessage = "Audience cannot be empty.")]
    public string Audience { get; init; } = null!;

    [Required, MinLength(1, ErrorMessage = "SecurityKey cannot be empty.")]
    public string SecurityKey { get; init; } = null!;

    [Required, Range(1, int.MaxValue, ErrorMessage = "AccessTokenExpirationMinutes must be greater than zero.")]
    public int AccessTokenExpirationMinutes { get; init; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "RefreshTokenExpirationMinutes must be greater than zero.")]
    public int RefreshTokenExpirationMinutes { get; init; }
}