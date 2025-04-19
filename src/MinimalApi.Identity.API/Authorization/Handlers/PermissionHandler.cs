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

//TODO: cleanup this class
public class PermissionHandler(ILogger<PermissionHandler> logger, UserManager<ApplicationUser> userManager) : IAuthorizationHandler
{
    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var user = context.User;
        var permissionsRequirements = context.Requirements.OfType<PermissionRequirement>();

        //if (user.Identity?.IsAuthenticated != true)
        //{
        //    var message = "User is not authenticated";

        //    logger.LogWarning(message);
        //    throw new UserUnknownException(message);
        //}

        //if (UsersExtensions.IsAuthenticated(user, logger))
        //{
        //    foreach (var permissionRequirement in permissionsRequirements)
        //    {
        //        if (permissionRequirement.Permissions.All(permission => user.HasClaim(CustomClaimTypes.Permission, permission)))
        //        {
        //            context.Succeed(permissionRequirement);
        //        }
        //        else
        //        {
        //            var message = $"User {user?.Identity?.Name} does not have the required permissions";

        //            logger.LogWarning(message);
        //            throw new UserWithoutPermissionsException(message);
        //        }
        //    }

        //    var userId = user.GetUserId();
        //    var utente = await userManager.FindByIdAsync(userId);
        //    var securityStamp = context.User.GetClaimValue(ClaimTypes.SerialNumber);

        //    if (utente == null)
        //    {
        //        var message = $"User {user?.Identity?.Name} not found";

        //        logger.LogWarning(message);
        //        throw new UserUnknownException(message);
        //    }
        //    else if (utente.LockoutEnd.GetValueOrDefault() > DateTimeOffset.UtcNow)
        //    {
        //        var message = MessageApi.UserLockedOut;

        //        logger.LogWarning(message);
        //        throw new UserIsLockedException(message);
        //    }
        //    else if (securityStamp != utente.SecurityStamp)
        //    {
        //        var message = $"User {user?.Identity?.Name} security stamp is invalid";

        //        logger.LogWarning(message);
        //        throw new UserTokenIsInvalidException(message);
        //    }
        //}

        if (UsersExtensions.IsAuthenticated(user))
        {
            var userId = user.GetUserId();
            var nameUser = user?.Identity?.Name;
            var securityStamp = context.User.GetClaimValue(ClaimTypes.SerialNumber);
            var utente = await userManager.FindByIdAsync(userId);

            if (utente == null || user == null)
            {
                var message = $"User {nameUser} not found";

                logger.LogWarning(message);
                throw new UserUnknownException(message);
            }

            if (utente.LockoutEnd.GetValueOrDefault() > DateTimeOffset.UtcNow)
            {
                var message = MessageApi.UserLockedOut;

                logger.LogWarning(message);
                throw new UserIsLockedException(message);
            }

            if (securityStamp != utente.SecurityStamp)
            {
                var message = $"User {nameUser} security stamp is invalid";

                logger.LogWarning(message);
                throw new UserTokenIsInvalidException(message);
            }

            foreach (var permissionRequirement in permissionsRequirements)
            {
                if (permissionRequirement.Permissions.All(permission => user.HasClaim(CustomClaimTypes.Permission, permission)))
                {
                    context.Succeed(permissionRequirement);
                }
                else
                {
                    var message = $"User {nameUser} does not have the required permissions";

                    logger.LogWarning(message);
                    throw new UserWithoutPermissionsException(message);
                }
            }
        }

        await Task.CompletedTask;
    }
}