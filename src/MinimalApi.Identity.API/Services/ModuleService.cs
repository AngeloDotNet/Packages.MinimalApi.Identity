using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Identity.API.Constants;
using MinimalApi.Identity.API.Database;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Services.Interfaces;

namespace MinimalApi.Identity.API.Services;

public class ModuleService(MinimalApiAuthDbContext dbContext, UserManager<ApplicationUser> userManager) : IModuleService
{
    public async Task<IResult> GetAllModulesAsync()
    {
        var query = await dbContext.Modules.ToListAsync();
        var result = query.Select(m => new ModuleResponseModel(m.Id, m.Name, m.Description)).ToList();

        return result == null ? TypedResults.NotFound(MessageApi.ModulesNotFound) : TypedResults.Ok(result);
    }

    public async Task<IResult> CreateModuleAsync(CreateModuleModel model)
    {
        var module = new Module
        {
            Name = model.Name,
            Description = model.Description
        };

        if (await CheckModuleExistAsync(model))
        {
            return TypedResults.Conflict(MessageApi.ModuleAlreadyExist);
        }

        dbContext.Modules.Add(module);
        await dbContext.SaveChangesAsync();

        return TypedResults.Ok(MessageApi.ModuleCreated);
    }

    public async Task<IResult> AssignModuleAsync(AssignModuleModel model)
    {
        var user = await userManager.FindByIdAsync(model.UserId.ToString());

        if (user == null)
        {
            return TypedResults.NotFound(MessageApi.UserNotFound);
        }

        var module = await dbContext.Modules.FindAsync(model.ModuleId);

        if (module == null)
        {
            return TypedResults.NotFound(MessageApi.ModuleNotFound);
        }

        var userHasModule = await dbContext.UserModules
            .Where(um => um.UserId == model.UserId)
            .Select(um => um.ModuleId)
            .ToListAsync();

        if (userHasModule.Contains(model.ModuleId))
        {
            return TypedResults.BadRequest(MessageApi.ModuleNotAssignable);
        }

        var userModule = new UserModule
        {
            UserId = model.UserId,
            ModuleId = model.ModuleId
        };

        dbContext.UserModules.Add(userModule);
        await dbContext.SaveChangesAsync();

        return TypedResults.Ok(MessageApi.ModuleAssigned);
    }

    public async Task<IResult> RevokeModuleAsync(RevokeModuleModel model)
    {
        var userModule = await dbContext.UserModules
            .SingleOrDefaultAsync(um => um.UserId == model.UserId && um.ModuleId == model.ModuleId);

        if (userModule == null)
        {
            return TypedResults.NotFound(MessageApi.ModuleNotFound);
        }

        dbContext.UserModules.Remove(userModule);
        await dbContext.SaveChangesAsync();

        return TypedResults.Ok(MessageApi.ModuleCanceled);
    }

    public async Task<IResult> DeleteModuleAsync(DeleteModuleModel model)
    {
        var module = await dbContext.Modules.FindAsync(model.ModuleId);

        if (module == null)
        {
            return TypedResults.NotFound(MessageApi.ModuleNotFound);
        }

        if (await dbContext.UserModules.AnyAsync(ul => ul.ModuleId == model.ModuleId))
        {
            return TypedResults.BadRequest(MessageApi.ModuleNotDeleted);
        }

        dbContext.Modules.Remove(module);
        await dbContext.SaveChangesAsync();

        return TypedResults.Ok(MessageApi.ModuleDeleted);
    }

    public async Task<IList<Claim>> GetClaimsModuleUserAsync(ApplicationUser user)
    {
        var moduleClaims = new List<Claim>();

        var result = await dbContext.UserModules
            .Where(ul => ul.UserId == user.Id)
            .Select(ul => ul.Module.Name)
            .ToListAsync();

        if (result.Any())
        {
            moduleClaims.AddRange(result.Select(moduleName => new Claim(CustomClaimTypes.Module, moduleName)));

            return moduleClaims;
        }

        return moduleClaims;
    }

    private async Task<bool> CheckModuleExistAsync(CreateModuleModel inputModel)
    {
        return await dbContext.Modules.AnyAsync(m => m.Name.Equals(inputModel.Name, StringComparison.InvariantCultureIgnoreCase));
    }
}
