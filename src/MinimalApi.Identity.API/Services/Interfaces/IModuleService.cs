using System.Security.Claims;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Models;

namespace MinimalApi.Identity.API.Services.Interfaces;

public interface IModuleService
{
    Task<List<ModuleResponseModel>> GetAllModulesAsync();
    Task<string> CreateModuleAsync(CreateModuleModel model);
    Task<string> AssignModuleAsync(AssignModuleModel model);
    Task<string> RevokeModuleAsync(RevokeModuleModel model);
    Task<string> DeleteModuleAsync(DeleteModuleModel model);

    //Task<IList<Claim>> GetClaimsModuleUserAsync(ApplicationUser user);
    Task<List<Claim>> GetClaimsModuleUserAsync(ApplicationUser user);
}