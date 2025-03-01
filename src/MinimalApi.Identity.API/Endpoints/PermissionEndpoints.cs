using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MinimalApi.Identity.API.Constants;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Enums;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.Common.Extensions.Interfaces;

namespace MinimalApi.Identity.API.Endpoints;

public class PermissionEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var apiGroup = endpoints
            .MapGroup(EndpointsApi.EndpointsPermissionsGroup)
            .RequireAuthorization()
            .WithOpenApi(opt =>
            {
                opt.Tags = [new OpenApiTag { Name = EndpointsApi.EndpointsPermissionsTag }];
                return opt;
            });

        apiGroup.MapGet(EndpointsApi.EndpointsStringEmpty, async Task<Results<Ok<List<Permission>>, NotFound<string>>>
            (MinimalApiDbContext dbContext) =>
        {
            var result = await dbContext.Permissions.ToListAsync();

            if (result == null)
            {
                return TypedResults.NotFound(MessageApi.PermissionsNotFound);
            }

            return TypedResults.Ok(result);
        })
        .RequireAuthorization(nameof(Authorization.GetPermissions))
        .WithOpenApi();

        apiGroup.MapPost(EndpointsApi.EndpointsCreatePermission, async (MinimalApiDbContext dbContext, [FromBody] Permission inputModel) =>
        {
            await dbContext.Permissions.AddAsync(inputModel);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok(MessageApi.PermissionCreated);
        })
        .RequireAuthorization(nameof(Authorization.CreatePermission))
        .WithOpenApi();

        apiGroup.MapPost(EndpointsApi.EndpointsAssignPermission, async Task<Results<Ok<string>, NotFound<string>>>
            (MinimalApiDbContext dbContext, [FromServices] RoleManager<IdentityRole> roleManager, [FromBody] AssignPermissionModel inputModel) =>
        {
            var role = await roleManager.FindByIdAsync(inputModel.RoleId.ToString());

            if (role == null)
            {
                return TypedResults.NotFound(MessageApi.RoleNotFound);
            }

            var permission = await dbContext.Permissions.FindAsync(inputModel.PermissionId);

            if (permission == null)
            {
                return TypedResults.NotFound(MessageApi.PermissionNotFound);
            }

            var rolePermission = new RolePermission
            {
                RoleId = inputModel.RoleId,
                PermissionId = inputModel.PermissionId
            };

            dbContext.RolePermissions.Add(rolePermission);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok(MessageApi.PermissionAssigned);
        })
        .RequireAuthorization(nameof(Authorization.AssignPermission))
        .WithOpenApi();

        apiGroup.MapDelete(EndpointsApi.EndpointsRevokePermission, async Task<Results<Ok<string>, NotFound<string>>>
            (MinimalApiDbContext dbContext, [FromServices] RoleManager<IdentityRole> roleManager, [FromBody] AssignPermissionModel inputModel) =>
        {
            var rolePermission = await dbContext.RolePermissions.SingleOrDefaultAsync(rp
                => rp.RoleId == inputModel.RoleId && rp.PermissionId == inputModel.PermissionId);

            if (rolePermission == null)
            {
                return TypedResults.NotFound(MessageApi.PermissionNotFound);
            }

            dbContext.RolePermissions.Remove(rolePermission);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok(MessageApi.PermissionCanceled);
        })
        .RequireAuthorization(nameof(Authorization.DeletePermission))
        .WithOpenApi();
    }
}
