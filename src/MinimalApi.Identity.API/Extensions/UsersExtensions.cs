using System.Security.Claims;
using System.Security.Principal;
using MinimalApi.Identity.API.Exceptions.Users;

namespace MinimalApi.Identity.API.Extensions;

public static class UsersExtensions
{
    public static string GetUserId(this IPrincipal user)
    {
        if (user is not ClaimsPrincipal claimsPrincipal)
        {
            throw new ArgumentException("User must be a ClaimsPrincipal", nameof(user));
        }

        var claimValue = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(claimValue))
        {
            throw new InvalidOperationException("Claim value for NameIdentifier is missing or empty.");
        }

        return claimValue;
    }

    public static string GetClaimValue(this IPrincipal user, string claimType) => ((ClaimsPrincipal)user).FindFirst(claimType)?.Value!;

    public static bool IsAuthenticated(this ClaimsPrincipal principal)
    {
        var userIsAuthenticated = principal.Identity?.IsAuthenticated ?? false;

        if (userIsAuthenticated == false)
        {
            throw new UserUnknownException("User is not authenticated");
        }

        return userIsAuthenticated;
    }
}
