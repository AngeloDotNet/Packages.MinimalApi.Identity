using MinimalApi.Identity.API.Enums;

namespace MinimalApi.Identity.API.Constants;

public static class CustomClaimTypes
{
    public const string FullName = nameof(ClaimsType.FullName);
    public const string License = nameof(ClaimsType.License);
    public const string Module = nameof(ClaimsType.Module);
    public const string Permission = nameof(ClaimsType.Permission);
}