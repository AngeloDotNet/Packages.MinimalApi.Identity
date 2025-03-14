using Microsoft.AspNetCore.Authorization;

namespace MinimalApi.Identity.BusinessLayer.Authorization.Requirement;

public class PermissionRequirement(params string[] permissions) : IAuthorizationRequirement
{
    public string[] Permissions { get; } = permissions;
}