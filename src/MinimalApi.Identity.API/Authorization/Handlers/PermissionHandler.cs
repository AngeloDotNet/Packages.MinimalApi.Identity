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

        if (user.Identity?.IsAuthenticated != true)
        {
            //if (context.Resource is HttpContext httpContext)
            //{
            //    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //}

            //context.Fail();
            //return;

            throw new UserUnknownException();
        }

        foreach (var requirement in context.Requirements)
        {
            if (requirement is PermissionRequirement permissionRequirement)
            {
                if (permissionRequirement.Permissions.All(permission => user.HasClaim(CustomClaimTypes.Permission, permission)))
                {
                    context.Succeed(permissionRequirement);
                }
                else
                {
                    //if (context.Resource is HttpContext httpContext)
                    //{
                    //    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    //}

                    //context.Fail();
                    //return;

                    throw new UserWithoutPermissionsException();
                }
            }
        }

        await Task.CompletedTask;
    }
}
