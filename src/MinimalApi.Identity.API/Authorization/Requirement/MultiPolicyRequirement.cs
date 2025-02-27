using Microsoft.AspNetCore.Authorization;

namespace MinimalApi.Identity.BusinessLayer.Authorization.Requirement;

public class MultiPolicyRequirement(params string[] policies) : IAuthorizationRequirement
{
    public string[] Policies { get; } = policies;
}