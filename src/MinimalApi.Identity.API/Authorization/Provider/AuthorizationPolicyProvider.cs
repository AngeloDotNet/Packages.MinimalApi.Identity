using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using MinimalApi.Identity.BusinessLayer.Authorization.Requirement;

namespace MinimalApi.Identity.BusinessLayer.Authorization.Provider;

public class AuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : DefaultAuthorizationPolicyProvider(options)
{
    private readonly AuthorizationOptions options = options.Value;

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        return await base.GetPolicyAsync(policyName)
            ?? new AuthorizationPolicyBuilder().AddRequirements(new AuthorizationRequirement(policyName)).Build();
    }
}