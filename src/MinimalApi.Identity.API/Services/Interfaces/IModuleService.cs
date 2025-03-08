using Microsoft.AspNetCore.Http;
using MinimalApi.Identity.API.Models;

namespace MinimalApi.Identity.API.Services.Interfaces;

public interface IModuleService
{
    Task<IResult> GetAllModulesAsync();
    Task<IResult> CreateModuleAsync(CreateModuleModel model);
    Task<IResult> AssignModuleAsync(AssignModuleModel model);
    Task<IResult> RevokeModuleAsync(RevokeModuleModel model);
}