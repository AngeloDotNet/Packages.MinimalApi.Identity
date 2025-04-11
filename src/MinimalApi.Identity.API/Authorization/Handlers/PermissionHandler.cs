using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MinimalApi.Identity.API.Constants;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Exceptions.Users;
using MinimalApi.Identity.API.Extensions;
using MinimalApi.Identity.BusinessLayer.Authorization.Requirement;

namespace MinimalApi.Identity.API.Authorization.Handlers;

public class PermissionHandler(ILogger<PermissionHandler> logger, UserManager<ApplicationUser> userManager) : IAuthorizationHandler
{
    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var user = context.User;
        var permissionsRequirements = context.Requirements.OfType<PermissionRequirement>();

        if (user.Identity?.IsAuthenticated != true)
        {
            var message = "User is not authenticated";

            logger.LogWarning(message);
            throw new UserUnknownException(message);
        }

        foreach (var permissionRequirement in permissionsRequirements)
        {
            if (permissionRequirement.Permissions.All(permission => user.HasClaim(CustomClaimTypes.Permission, permission)))
            {
                context.Succeed(permissionRequirement);
            }
            else
            {
                var message = $"User {user?.Identity?.Name} does not have the required permissions";

                logger.LogWarning(message);
                throw new UserWithoutPermissionsException(message);
            }
        }

        var userId = user.GetClaimValue(ClaimTypes.NameIdentifier);
        var utente = await userManager.FindByIdAsync(userId);
        var securityStamp = context.User.GetClaimValue(ClaimTypes.SerialNumber);

        if (utente == null)
        {
            var message = $"User {user?.Identity?.Name} not found";

            logger.LogWarning(message);
            throw new UserUnknownException(message);
        }
        //else if (utente.LockoutEnd.GetValueOrDefault() > DateTimeOffset.UtcNow || securityStamp != utente.SecurityStamp)
        else if (utente.LockoutEnd.GetValueOrDefault() > DateTimeOffset.UtcNow)
        {
            //var message = $"User {user?.Identity?.Name} is locked out";
            var message = MessageApi.UserLockedOut;

            logger.LogWarning(message);
            throw new UserIsLockedException(message);
        }
        else if (securityStamp != utente.SecurityStamp)
        {
            var message = $"User {user?.Identity?.Name} security stamp is invalid";
            logger.LogWarning(message);

            throw new UserTokenIsInvalidException(message);
        }

        await Task.CompletedTask;
    }
}