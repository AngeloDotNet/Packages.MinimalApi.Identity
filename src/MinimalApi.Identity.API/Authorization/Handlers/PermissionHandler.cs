using Microsoft.AspNetCore.Authorization;
using MinimalApi.Identity.API.Constants;
using MinimalApi.Identity.API.Exceptions;
using MinimalApi.Identity.BusinessLayer.Authorization.Requirement;

namespace MinimalApi.Identity.API.Authorization.Handlers;

public class PermissionHandler : IAuthorizationHandler
{
    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var user = context.User;
        var permissionsRequirements = context.Requirements.OfType<PermissionRequirement>().ToList();

        if (user.Identity?.IsAuthenticated != true)
        {
            throw new UserUnknownException();
        }

        if (!context.Requirements.Any())
        {
            throw new UserWithoutPermissionsException();
        }

        foreach (var permissionRequirement in permissionsRequirements)
        {
            if (permissionRequirement.Permissions.All(permission => user.HasClaim(CustomClaimTypes.Permission, permission)))
            {
                context.Succeed(permissionRequirement);
            }
            else
            {
                throw new UserWithoutPermissionsException();
            }
        }

        await Task.CompletedTask;
    }
}