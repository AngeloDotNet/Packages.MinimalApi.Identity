using System.ComponentModel.DataAnnotations;
using MinimalApi.Identity.API.Validator.Attribute;

namespace MinimalApi.Identity.API.Options;

public class NetIdentityOptions
{
    public bool RequireUniqueEmail { get; init; }
    public bool RequireDigit { get; init; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "RequiredLength must be greater than zero.")]
    public int RequiredLength { get; init; }

    public bool RequireUppercase { get; init; }
    public bool RequireLowercase { get; init; }
    public bool RequireNonAlphanumeric { get; init; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "RequiredUniqueChars must be greater than zero.")]
    public int RequiredUniqueChars { get; init; }
    public bool RequireConfirmedEmail { get; init; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "MaxFailedAccessAttempts must be greater than zero.")]
    public int MaxFailedAccessAttempts { get; init; }

    public bool AllowedForNewUsers { get; init; }

    [TimeSpanRange("00:00:01", "1.00:00:00", ErrorMessage = "DefaultLockoutTimeSpan must be between 1 second and 1 day.")]
    public TimeSpan DefaultLockoutTimeSpan { get; init; }
}