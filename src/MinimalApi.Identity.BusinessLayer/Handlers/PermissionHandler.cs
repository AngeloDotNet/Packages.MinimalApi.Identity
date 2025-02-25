using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace MinimalApi.Identity.BusinessLayer.Handlers;

public class PermissionHandler : AuthorizationHandler<AuthorizationRequirement>
{
    //private readonly IDataAccessService _dataAccessService;

    //public PermissionHandler(IDataAccessService dataAccessService)
    //{
    //    _dataAccessService = dataAccessService;
    //}

    protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationRequirement requirement)
    {
        //if (context.Resource is HttpContext httpContext && httpContext.GetEndpoint() is RouteEndpoint endpoint)
        if (context.Resource is HttpContext httpContext)
        {
            //endpoint.RoutePattern.RequiredValues.TryGetValue("controller", out var _controller);
            //endpoint.RoutePattern.RequiredValues.TryGetValue("action", out var _action);

            //endpoint.RoutePattern.RequiredValues.TryGetValue("page", out var _page);
            //endpoint.RoutePattern.RequiredValues.TryGetValue("area", out var _area);

            // Check if a parent action is permitted then it'll allow child without checking for child permissions
            if (!string.IsNullOrWhiteSpace(requirement?.PermissionName) && !requirement.PermissionName.Equals("Authorization"))
            {
                //_action = requirement.PermissionName;
            }

            //if (requirement != null && context.User.Identity?.IsAuthenticated == true && _controller != null && _action != null &&
            //    await _dataAccessService.GetMenuItemsAsync(context.User, _controller.ToString(), _action.ToString()))

            //if (requirement != null && context.User.Identity?.IsAuthenticated == true && _controller != null && _action != null)
            if (requirement != null && context.User.Identity!.IsAuthenticated == true)
            {
                context.Succeed(requirement);
            }
        }

        await Task.CompletedTask;
    }
}