using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace MinimalApi.Identity.BusinessLayer.Handlers;

public class AuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : DefaultAuthorizationPolicyProvider(options)
{
    private readonly AuthorizationOptions options = options.Value;

    public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
        return await base.GetPolicyAsync(policyName) ?? new AuthorizationPolicyBuilder()
            .AddRequirements(new AuthorizationRequirement(policyName)).Build();
    }
}