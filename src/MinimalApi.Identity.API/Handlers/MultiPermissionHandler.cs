using Microsoft.AspNetCore.Authorization;
using MinimalApi.Identity.API.Extensions;
using MinimalApi.Identity.BusinessLayer.Authorization.Requirement;

namespace MinimalApi.Identity.API.Handlers;

public class MultiPermissionHandler : IAuthorizationHandler
{
    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var confirmedPolicies = true;

        foreach (var requirement in context.Requirements.OfType<MultiPolicyRequirement>().ToList())
        {
            if (context.User.Identity!.IsAuthenticated == true)
            {
                foreach (var permission in requirement.Policies)
                {
                    if (!context.User.Claims.Any(claim => claim.Type == ClaimsExtensions.Permission && claim.Value == permission))
                    {
                        confirmedPolicies = false;
                        break;
                    }
                }

                if (confirmedPolicies)
                {
                    await Task.CompletedTask;

                    context.Succeed(requirement);
                }
            }
        }
    }
}
