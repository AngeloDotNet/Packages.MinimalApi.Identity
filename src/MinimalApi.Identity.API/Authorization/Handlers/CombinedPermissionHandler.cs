using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using MinimalApi.Identity.API.Extensions;
using MinimalApi.Identity.BusinessLayer.Authorization.Requirement;

namespace MinimalApi.Identity.API.Authorization.Handlers;

public class CombinedPermissionHandler : IAuthorizationHandler
{
    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var confirmedPolicies = true;

        foreach (var requirement in context.Requirements)
        {
            if (requirement is MultiPolicyRequirement multiPolicyRequirement)
            {
                if (context.User.Identity!.IsAuthenticated == true)
                {
                    foreach (var permission in multiPolicyRequirement.Policies)
                    {
                        if (!context.User.Claims.Any(claim => claim.Type == CustomClaimTypes.Permission && claim.Value == permission))
                        {
                            confirmedPolicies = false;
                            break;
                        }
                    }

                    if (confirmedPolicies)
                    {
                        context.Succeed(multiPolicyRequirement);
                    }
                }
            }
            else if (requirement is AuthorizationRequirement authorizationRequirement)
            {
                var permission = authorizationRequirement.Permission;

                if (context.Resource is HttpContext httpContext)
                {
                    if (context.User.Identity!.IsAuthenticated == true && !string.IsNullOrWhiteSpace(permission))
                    {
                        if (!context.User.Claims.Any(claim => claim.Type == CustomClaimTypes.Permission && claim.Value == permission))
                        {
                            context.Fail();
                        }
                        else
                        {
                            context.Succeed(authorizationRequirement);
                        }
                    }
                }
            }
        }

        await Task.CompletedTask;
    }
}