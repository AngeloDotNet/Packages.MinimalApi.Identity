using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace MinimalApi.Identity.BusinessLayer.Handlers;

public class PermissionHandler : AuthorizationHandler<AuthorizationRequirement>
{
    protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationRequirement requirement)
    {
        if (context.Resource is HttpContext httpContext)
        {
            var permission = string.Empty;

            if (!string.IsNullOrWhiteSpace(requirement?.PermissionName))
            {
                permission = requirement.PermissionName;
            }

            if (requirement != null && context.User.Identity!.IsAuthenticated == true && permission != null)
            {
                if (context.User.Claims.Any(c => c.Type == "Permission" && c.Value == permission))
                {
                    context.Succeed(requirement);
                }

                context.Fail();
            }
        }

        await Task.CompletedTask;
    }
}