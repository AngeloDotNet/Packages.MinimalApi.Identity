using Microsoft.AspNetCore.Authorization;

namespace MinimalApi.Identity.BusinessLayer.Handlers;

public class AuthorizationRequirement(string permissionName) : IAuthorizationRequirement
{
    public string PermissionName { get; } = permissionName;
}
