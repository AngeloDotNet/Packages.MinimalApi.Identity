using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Identity.API.Constants;
using MinimalApi.Identity.API.Database;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Exceptions;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Services.Interfaces;

namespace MinimalApi.Identity.API.Services;

public class ModuleService(MinimalApiAuthDbContext dbContext, UserManager<ApplicationUser> userManager) : IModuleService
{
    public async Task<List<ModuleResponseModel>> GetAllModulesAsync()
    {
        var result = await dbContext.Modules
            .Select(m => new ModuleResponseModel(m.Id, m.Name, m.Description))
            .ToListAsync();

        if (result.Count == 0)
        {
            throw new NotFoundModuleException(MessageApi.ModulesNotFound);
        }

        return result;
    }

    public async Task<string> CreateModuleAsync(CreateModuleModel model)
    {
        if (await CheckModuleExistAsync(model))
        {
            throw new ConflictModuleException(MessageApi.ModuleAlreadyExist);
        }

        var module = new Module
        {
            Name = model.Name,
            Description = model.Description
        };

        dbContext.Modules.Add(module);
        await dbContext.SaveChangesAsync();

        return MessageApi.ModuleCreated;
    }

    public async Task<string> AssignModuleAsync(AssignModuleModel model)
    {
        var user = await userManager.FindByIdAsync(model.UserId.ToString())
            ?? throw new NotFoundUserException(MessageApi.UserNotFound);

        var module = await dbContext.Modules.FindAsync(model.ModuleId)
            ?? throw new NotFoundModuleException(MessageApi.ModuleNotFound);

        var userHasModule = await dbContext.UserModules
            .AnyAsync(um => um.UserId == model.UserId && um.ModuleId == model.ModuleId);

        if (userHasModule)
        {
            throw new BadRequestModuleException(MessageApi.ModuleNotAssignable);
        }

        var userModule = new UserModule
        {
            UserId = model.UserId,
            ModuleId = model.ModuleId
        };

        dbContext.UserModules.Add(userModule);
        await dbContext.SaveChangesAsync();

        return MessageApi.ModuleAssigned;
    }

    public async Task<string> RevokeModuleAsync(RevokeModuleModel model)
    {
        var userModule = await dbContext.UserModules
            .SingleOrDefaultAsync(um => um.UserId == model.UserId && um.ModuleId == model.ModuleId)
            ?? throw new NotFoundModuleException(MessageApi.ModuleNotFound);

        dbContext.UserModules.Remove(userModule);
        await dbContext.SaveChangesAsync();

        return MessageApi.ModuleCanceled;
    }

    public async Task<string> DeleteModuleAsync(DeleteModuleModel model)
    {
        var module = await dbContext.Modules.FindAsync(model.ModuleId)
            ?? throw new NotFoundModuleException(MessageApi.ModuleNotFound);

        if (await dbContext.UserModules.AnyAsync(ul => ul.ModuleId == model.ModuleId))
        {
            throw new BadRequestModuleException(MessageApi.ModuleNotDeleted);
        }

        dbContext.Modules.Remove(module);
        await dbContext.SaveChangesAsync();

        return MessageApi.ModuleDeleted;
    }

    public async Task<List<Claim>> GetClaimsModuleUserAsync(ApplicationUser user)
    {
        var result = await dbContext.UserModules
            .Where(ul => ul.UserId == user.Id)
            .Select(ul => ul.Module.Name)
            .ToListAsync();

        return result.Select(moduleName => new Claim(CustomClaimTypes.Module, moduleName)).ToList();
    }

    private async Task<bool> CheckModuleExistAsync(CreateModuleModel inputModel)
    {
        return await dbContext.Modules.AnyAsync(m => m.Name.Equals(inputModel.Name, StringComparison.InvariantCultureIgnoreCase));
    }
}
