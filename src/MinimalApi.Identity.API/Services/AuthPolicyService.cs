using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MinimalApi.Identity.API.Constants;
using MinimalApi.Identity.API.Database;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Exceptions;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Services.Interfaces;
using MinimalApi.Identity.BusinessLayer.Authorization.Requirement;

namespace MinimalApi.Identity.API.Services;

public class AuthPolicyService(MinimalApiAuthDbContext dbContext, ILogger<AuthPolicyService> logger,
    IServiceProvider serviceProvider) : IAuthPolicyService
{
    public async Task<IResult> GetAllPoliciesAsync()
    {
        var query = await dbContext.AuthPolicies.AsNoTracking().ToListAsync();

        if (query == null || query.Count == 0)
        {
            return TypedResults.NotFound(MessageApi.PolicyNotFound);
        }

        var result = query.Select(p
            => new PolicyResponseModel(p.Id, p.PolicyName, p.PolicyDescription, p.PolicyPermissions)).ToList();

        return TypedResults.Ok(result);
    }

    public async Task<IResult> CreatePolicyAsync(CreatePolicyModel model)
    {
        var authPolicy = new AuthPolicy
        {
            PolicyName = model.PolicyName,
            PolicyDescription = model.PolicyDescription,
            PolicyPermissions = model.PolicyPermissions,
            IsDefault = false,
            IsActive = true
        };

        if (await CheckPolicyExistAsync(model))
        {
            return TypedResults.Conflict(MessageApi.PolicyAlreadyExist);
        }

        dbContext.AuthPolicies.Add(authPolicy);
        await dbContext.SaveChangesAsync();

        return TypedResults.Ok(MessageApi.PolicyCreated);
    }

    public async Task<IResult> DeletePolicyAsync(DeletePolicyModel model)
    {
        var authPolicy = await dbContext.AuthPolicies.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == model.Id && x.PolicyName == model.PolicyName);

        if (authPolicy == null)
        {
            return TypedResults.NotFound(MessageApi.PolicyNotFound);
        }

        if (authPolicy.IsDefault)
        {
            return TypedResults.BadRequest(MessageApi.PolicyNotDeleted);
        }

        dbContext.AuthPolicies.Remove(authPolicy);
        await dbContext.SaveChangesAsync();

        return TypedResults.Ok(MessageApi.PolicyDeleted);
    }

    public async Task<List<AuthPolicy>> GetAllAuthPoliciesAsync(Expression<Func<AuthPolicy, bool>> filter = null!)
    {
        var query = dbContext.AuthPolicies.AsNoTracking();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.OrderBy(x => x.Id).ToListAsync();
    }

    public async Task<bool> GenerateAuthPoliciesAsync()
    {
        try
        {
            using var scope = serviceProvider.CreateScope();

            var authorizationPolicyService = scope.ServiceProvider.GetRequiredService<IAuthPolicyService>();
            var listPolicy = await authorizationPolicyService.GetAllAuthPoliciesAsync(x => x.IsActive);
            var authorizationOptions = serviceProvider.GetRequiredService<IOptions<AuthorizationOptions>>().Value;

            if (listPolicy is null || listPolicy.Count == 0)
            {
                logger.LogWarning("No active policies found in the database.");
                throw new NotFoundActivePoliciesException();
            }

            logger.LogInformation("Found {PolicyCount} active policies in the database.", listPolicy.Count);

            foreach (var policy in listPolicy)
            {
                if (authorizationOptions.GetPolicy(policy.PolicyName) != null)
                {
                    logger.LogInformation("Policy {PolicyName} already exists.", policy.PolicyName);
                    continue;
                }

                var permissionRequirement = new PermissionRequirement(policy.PolicyPermissions);
                logger.LogInformation("Adding policy: {PolicyName} with permissions: {PolicyPermissions}", policy.PolicyName, string.Join(", ", policy.PolicyPermissions));
                authorizationOptions.AddPolicy(policy.PolicyName, policy => policy.Requirements.Add(permissionRequirement));
            }

            return true;
        }
        catch (NotFoundActivePoliciesException ex)
        {
            logger.LogWarning(ex, "No active policies found in the database.");
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while generating authorization policies.");
            return false;
        }
    }

    public async Task<bool> UpdateAuthPoliciesAsync()
    {
        try
        {
            using var scope = serviceProvider.CreateScope();

            var authorizationPolicyService = scope.ServiceProvider.GetRequiredService<IAuthPolicyService>();
            var listPolicy = await authorizationPolicyService.GetAllAuthPoliciesAsync(x => x.IsActive);
            var authorizationOptions = serviceProvider.GetRequiredService<IOptions<AuthorizationOptions>>().Value;

            if (listPolicy is null || listPolicy.Count == 0)
            {
                logger.LogWarning("No active policies found in the database.");
                throw new NotFoundActivePoliciesException();
            }

            logger.LogInformation("Found {PolicyCount} active policies in the database.", listPolicy.Count);
            foreach (var policy in listPolicy)
            {
                if (authorizationOptions.GetPolicy(policy.PolicyName) != null)
                {
                    logger.LogInformation("Policy {PolicyName} already exists.", policy.PolicyName);
                    continue;
                }

                var permissionRequirement = new PermissionRequirement(policy.PolicyPermissions);
                logger.LogInformation("Adding policy: {PolicyName} with permissions: {PolicyPermissions}", policy.PolicyName, string.Join(", ", policy.PolicyPermissions));
                authorizationOptions.AddPolicy(policy.PolicyName, policy => policy.Requirements.Add(permissionRequirement));
            }

            return true;
        }
        catch (NotFoundActivePoliciesException ex)
        {
            logger.LogWarning(ex, "No active policies found in the database.");
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while generating authorization policies.");
            return false;
        }
    }

    private async Task<bool> CheckPolicyExistAsync(CreatePolicyModel model)
    {
        return await dbContext.AuthPolicies.AsNoTracking()
            .AnyAsync(x => x.PolicyName.Equals(model.PolicyName, StringComparison.InvariantCultureIgnoreCase)
            || x.PolicyPermissions.SequenceEqual(model.PolicyPermissions));
    }
}