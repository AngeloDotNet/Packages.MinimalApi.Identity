using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using MinimalApi.Identity.API.Constants;
using MinimalApi.Identity.API.Exceptions;
using MinimalApi.Identity.BusinessLayer.Authorization.Requirement;

namespace MinimalApi.Identity.API.Authorization.Handlers;

public class PermissionHandler(ILogger<PermissionHandler> logger) : IAuthorizationHandler
{
    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var user = context.User;
        var permissionsRequirements = context.Requirements.OfType<PermissionRequirement>();

        if (user.Identity?.IsAuthenticated != true)
        {
            logger.LogWarning("User is not authenticated");
            throw new UserUnknownException();
        }

        foreach (var permissionRequirement in permissionsRequirements)
        {
            if (permissionRequirement.Permissions.All(permission => user.HasClaim(CustomClaimTypes.Permission, permission)))
            {
                context.Succeed(permissionRequirement);
            }
            else
            {
                logger.LogWarning("User {UserName} does not have the required permissions", user?.Identity?.Name);
                throw new UserWithoutPermissionsException();
            }
        }

        await Task.CompletedTask;
    }
}
