using Microsoft.AspNetCore.Authorization;

namespace MinimalApi.Identity.BusinessLayer.Authorization.Requirement;

public class AuthorizationRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}