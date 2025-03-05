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

            //if (result == null)
            //{
            //    return TypedResults.NotFound(MessageApi.PermissionsNotFound);
            //}

            //return TypedResults.Ok(result);

            return result == null ? TypedResults.NotFound(MessageApi.PermissionsNotFound) : TypedResults.Ok(result);
        })
        .Produces<Ok<List<Permission>>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(nameof(Authorization.GetPermissions))
        .WithOpenApi(opt =>
        {
            opt.Summary = "Get all permissions";
            opt.Description = "Get all permissions";
            return opt;
        });

        apiGroup.MapPost(EndpointsApi.EndpointsCreatePermission, async (MinimalApiDbContext dbContext, [FromBody] Permission inputModel) =>
        {
            await dbContext.Permissions.AddAsync(inputModel);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok(MessageApi.PermissionCreated);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(nameof(Authorization.CreatePermission))
        .WithOpenApi(opt =>
        {
            opt.Summary = "Create a new permission";
            opt.Description = "Create a new permission";
            return opt;
        });

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
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(nameof(Authorization.AssignPermission))
        .WithOpenApi(opt =>
        {
            opt.Summary = "Assign a permission to a role";
            opt.Description = "Assign a permission to a role";
            return opt;
        });

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
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(nameof(Authorization.DeletePermission))
        .WithOpenApi(opt =>
        {
            opt.Summary = "Revoke a permission from a role";
            opt.Description = "Revoke a permission from a role";
            return opt;
        });
    }
}
