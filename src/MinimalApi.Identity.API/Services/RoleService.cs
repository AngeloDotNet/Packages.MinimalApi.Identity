using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Identity.API.Constants;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Exceptions.BadRequest;
using MinimalApi.Identity.API.Exceptions.Conflict;
using MinimalApi.Identity.API.Exceptions.NotFound;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Services.Interfaces;

namespace MinimalApi.Identity.API.Services;

public class RoleService(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager) : IRoleService
{
    public async Task<List<RoleResponseModel>> GetAllRolesAsync()
    {
        var roles = await roleManager.Roles
            .Select(r => new RoleResponseModel(r.Id, r.Name!, r.Default))
            .ToListAsync();

        if (roles.Count == 0)
        {
            throw new NotFoundRoleException(MessageApi.RolesNotFound);
        }

        return roles;
    }

    public async Task<string> CreateRoleAsync(CreateRoleModel model)
    {
        if (await roleManager.RoleExistsAsync(model.Role))
        {
            throw new ConflictRoleException(MessageApi.RoleExists);
        }

        var newRole = new ApplicationRole
        {
            Name = model.Role,
            Default = false
        };

        var result = await roleManager.CreateAsync(newRole);

        if (!result.Succeeded)
        {
            throw new BadRequestRoleException(result.Errors);
        }

        return MessageApi.RoleCreated;
    }

    public async Task<string> AssignRoleAsync(AssignRoleModel model)
    {
        var user = await userManager.FindByNameAsync(model.Username)
            ?? throw new NotFoundUserException(MessageApi.UserNotFound);

        if (!await roleManager.RoleExistsAsync(model.Role))
        {
            throw new NotFoundRoleException(MessageApi.RoleNotFound);
        }

        var result = await userManager.AddToRoleAsync(user, model.Role);

        if (!result.Succeeded)
        {
            throw new BadRequestRoleException(result.Errors);
        }

        return MessageApi.RoleAssigned;
    }

    public async Task<string> RevokeRoleAsync(RevokeRoleModel model)
    {
        var user = await userManager.FindByNameAsync(model.Username)
            ?? throw new NotFoundUserException(MessageApi.UserNotFound);

        var result = await userManager.RemoveFromRoleAsync(user, model.Role);

        if (!result.Succeeded)
        {
            throw new BadRequestRoleException(result.Errors);
        }

        return MessageApi.RoleCanceled;
    }

    public async Task<string> DeleteRoleAsync(DeleteRoleModel model)
    {
        var role = await roleManager.FindByNameAsync(model.Role)
            ?? throw new NotFoundRoleException(MessageApi.RoleNotFound);

        if (role.Default)
        {
            throw new BadRequestRoleException(MessageApi.RoleNotDeleted);
        }

        var result = await roleManager.DeleteAsync(role);

        if (!result.Succeeded)
        {
            throw new BadRequestRoleException(result.Errors);
        }

        return MessageApi.RoleDeleted;
    }
}
