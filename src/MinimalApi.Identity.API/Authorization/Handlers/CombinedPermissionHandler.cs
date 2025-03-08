using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using MinimalApi.Identity.API.Constants;
using MinimalApi.Identity.BusinessLayer.Authorization.Requirement;

namespace MinimalApi.Identity.API.Authorization.Handlers;

public class CombinedPermissionHandler : IAuthorizationHandler
{
    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var user = context.User;

        if (user.Identity?.IsAuthenticated != true)
        {
            if (context.Resource is HttpContext httpContext)
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }

            context.Fail();
        }

        foreach (var requirement in context.Requirements)
        {
            switch (requirement)
            {
                case MultiPolicyRequirement multiPolicyRequirement:

                    if (multiPolicyRequirement.Policies.All(permission => user.HasClaim(CustomClaimTypes.Permission, permission)))
                    {
                        context.Succeed(multiPolicyRequirement);
                    }
                    else
                    {
                        if (context.Resource is HttpContext httpContext)
                        {
                            httpContext.Response.StatusCode = StatusCodes.Status412PreconditionFailed;
                        }

                        context.Fail();
                    }

                    break;

                case AuthorizationRequirement authorizationRequirement:

                    if (context.Resource is HttpContext && !string.IsNullOrWhiteSpace(authorizationRequirement.Permission))
                    {
                        if (user.HasClaim(CustomClaimTypes.Permission, authorizationRequirement.Permission))
                        {
                            context.Succeed(authorizationRequirement);
                        }
                        else
                        {
                            if (context.Resource is HttpContext httpContext)
                            {
                                httpContext.Response.StatusCode = StatusCodes.Status412PreconditionFailed;
                            }

                            context.Fail();
                        }
                    }

                    break;
            }
        }

        await Task.CompletedTask;
    }
}