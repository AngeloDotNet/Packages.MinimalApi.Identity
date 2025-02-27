using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using MinimalApi.Identity.API.Extensions;
using MinimalApi.Identity.BusinessLayer.Authorization.Requirement;

namespace MinimalApi.Identity.API.Handlers;

public class PermissionHandler : AuthorizationHandler<AuthorizationRequirement>
{
    protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationRequirement requirement)
    {
        var permission = string.Empty;

        if (context.Resource is HttpContext httpContext)
        {
            if (!string.IsNullOrWhiteSpace(requirement?.Permission))
            {
                permission = requirement.Permission;
            }

            if (requirement != null && context.User.Identity!.IsAuthenticated == true && permission != null)
            {
                if (!context.User.Claims.Any(claim => claim.Type == ClaimsExtensions.Permission && claim.Value == permission))
                {
                    context.Fail();
                }

                context.Succeed(requirement);
            }
        }

        await Task.CompletedTask;
    }
}