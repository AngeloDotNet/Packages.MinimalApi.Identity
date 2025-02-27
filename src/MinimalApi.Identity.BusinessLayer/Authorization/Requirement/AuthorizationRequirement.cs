using Microsoft.AspNetCore.Authorization;

namespace MinimalApi.Identity.BusinessLayer.Authorization.Requirement;

public class AuthorizationRequirement(string permissionName) : IAuthorizationRequirement
{
    public string PermissionName { get; } = permissionName;
}