namespace MinimalApi.Identity.API.Options;

public class NetIdentityOptions
{
    public bool RequireUniqueEmail { get; init; }
    public bool RequireDigit { get; init; }
    public int RequiredLength { get; init; }
    public bool RequireUppercase { get; init; }
    public bool RequireLowercase { get; init; }
    public bool RequireNonAlphanumeric { get; init; }
    public int RequiredUniqueChars { get; init; }
    public bool RequireConfirmedEmail { get; init; }
    public int MaxFailedAccessAttempts { get; init; }
    public bool AllowedForNewUsers { get; init; }
    public TimeSpan DefaultLockoutTimeSpan { get; init; }
}