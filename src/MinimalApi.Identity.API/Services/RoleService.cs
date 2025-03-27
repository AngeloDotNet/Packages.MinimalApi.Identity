﻿using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Identity.API.Constants;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Services.Interfaces;

namespace MinimalApi.Identity.API.Services;

public class RoleService(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager) : IRoleService
{
    public async Task<IResult> GetAllRolesAsync()
    {
        var roles = await roleManager.Roles
            .Select(r => new RoleResponseModel(r.Id, r.Name!, r.Default))
            .ToListAsync();

        return roles.Count == 0 ? TypedResults.NotFound(MessageApi.RolesNotFound) : TypedResults.Ok(roles);
    }

    public async Task<IResult> CreateRoleAsync(CreateRoleModel model)
    {
        if (await roleManager.RoleExistsAsync(model.Role))
        {
            return TypedResults.Conflict(MessageApi.RoleExists);
        }

        var newRole = new ApplicationRole
        {
            Name = model.Role,
            Default = false
        };

        var result = await roleManager.CreateAsync(newRole);

        return result.Succeeded ? TypedResults.Ok(MessageApi.RoleCreated) : TypedResults.BadRequest(result.Errors);
    }

    public async Task<IResult> AssignRoleAsync(AssignRoleModel model)
    {
        var user = await userManager.FindByNameAsync(model.Username);
        if (user == null)
        {
            return TypedResults.NotFound(MessageApi.UserNotFound);
        }

        if (!await roleManager.RoleExistsAsync(model.Role))
        {
            return TypedResults.NotFound(MessageApi.RoleNotFound);
        }

        var result = await userManager.AddToRoleAsync(user, model.Role);

        return result.Succeeded ? TypedResults.Ok(MessageApi.RoleAssigned) : TypedResults.BadRequest(result.Errors);
    }

    public async Task<IResult> RevokeRoleAsync(RevokeRoleModel model)
    {
        var user = await userManager.FindByNameAsync(model.Username);
        if (user == null)
        {
            return TypedResults.NotFound(MessageApi.UserNotFound);
        }

        var result = await userManager.RemoveFromRoleAsync(user, model.Role);

        return result.Succeeded ? TypedResults.Ok(MessageApi.RoleCanceled) : TypedResults.BadRequest(result.Errors);
    }

    public async Task<IResult> DeleteRoleAsync(DeleteRoleModel model)
    {
        var role = await roleManager.FindByNameAsync(model.Role);
        if (role == null)
        {
            return TypedResults.NotFound(MessageApi.RoleNotFound);
        }

        if (role.Default)
        {
            return TypedResults.BadRequest(MessageApi.RoleNotDeleted);
        }

        var result = await roleManager.DeleteAsync(role);

        return result.Succeeded ? TypedResults.Ok(MessageApi.RoleDeleted) : TypedResults.BadRequest(result.Errors);
    }
}
