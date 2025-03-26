using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Models;

namespace MinimalApi.Identity.API.Services.Interfaces;

public interface IAuthPolicyService
{
    Task<IResult> GetAllPoliciesAsync();
    Task<IResult> CreatePolicyAsync(CreatePolicyModel model);
    Task<IResult> DeletePolicyAsync(DeletePolicyModel model);

    Task<bool> GenerateAuthPoliciesAsync();
    Task<bool> UpdateAuthPoliciesAsync();
    Task<List<AuthPolicy>> GetAllAuthPoliciesAsync(Expression<Func<AuthPolicy, bool>> filter = null!);
}