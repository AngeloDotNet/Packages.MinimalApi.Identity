using Microsoft.AspNetCore.Authorization;
using MinimalApi.Identity.BusinessLayer.Authorization.Requirement;
using MinimalApi.Identity.BusinessLayer.Extensions;

namespace MinimalApi.Identity.BusinessLayer.Handlers;

public class MultiPermissionHandler : IAuthorizationHandler
{
    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var requirements = context.Requirements.OfType<MultiPolicyRequirement>().ToList();

        foreach (var requirement in requirements)
        {
            var allPoliciesSatisfied = true;

            if (context.User.Identity!.IsAuthenticated == true)
            {
                foreach (var permission in requirement.Policies)
                {
                    if (!context.User.Claims.Any(c => c.Type == ClaimsExtensions.Permission && c.Value == permission))
                    {
                        allPoliciesSatisfied = false;
                        break;
                    }
                }

                if (allPoliciesSatisfied)
                {
                    await Task.CompletedTask;

                    context.Succeed(requirement);
                }
            }

            //foreach (var permission in requirement.Policies)
            //{
            //    if (!context.User.Claims.Any(c => c.Type == ClaimsExtensions.Permission && c.Value == permission))
            //    {
            //        allPoliciesSatisfied = false;
            //        break;
            //    }
            //}

            //if (allPoliciesSatisfied)
            //{
            //    context.Succeed(requirement);
            //}
        }
    }
}
