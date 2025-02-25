using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MinimalApi.Identity.Common.Extensions.Interfaces;
using MinimalApi.Identity.DataAccessLayer;
using MinimalApi.Identity.DataAccessLayer.Entities;
using MinimalApi.Identity.Shared;

namespace MinimalApi.Identity.API.Endpoints;

public class PermissionEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var apiGroup = endpoints
            .MapGroup("/permessi")
            .WithOpenApi(opt =>
            {
                opt.Tags = [new OpenApiTag { Name = "Permessi" }];
                return opt;
            });

        apiGroup.MapGet(string.Empty, async Task<Results<Ok<List<Permission>>, BadRequest>> (MinimalApiDbContext dbContext) =>
        {
            var result = await dbContext.Permissions.ToListAsync();

            if (result == null)
            {
                return TypedResults.BadRequest();
            }

            return TypedResults.Ok(result);
        });

        apiGroup.MapPost("/crea-permesso", async (MinimalApiDbContext dbContext, [FromBody] Permission inputModel) =>
        {
            await dbContext.Permissions.AddAsync(inputModel);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok();
        });

        apiGroup.MapPost("/assegna-permesso", async Task<Results<Ok, NotFound<string>>> (MinimalApiDbContext dbContext,
           [FromServices] RoleManager<IdentityRole> roleManager, [FromBody] AssignPermissionModel inputModel) =>
        {
            var role = await roleManager.FindByIdAsync(inputModel.RoleId.ToString());

            if (role == null)
            {
                return TypedResults.NotFound("Role not found");
            }

            var permission = await dbContext.Permissions.FindAsync(inputModel.PermissionId);

            if (permission == null)
            {
                return TypedResults.NotFound("Permission not found");
            }

            var rolePermission = new RolePermission
            {
                RoleId = inputModel.RoleId,
                PermissionId = inputModel.PermissionId
            };

            dbContext.RolePermissions.Add(rolePermission);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok();
        });

        apiGroup.MapDelete("/rimuovi-permesso", async Task<Results<Ok, NotFound<string>>> (MinimalApiDbContext dbContext,
            [FromServices] RoleManager<IdentityRole> roleManager, [FromBody] AssignPermissionModel inputModel) =>
        {
            var rolePermission = await dbContext.RolePermissions.SingleOrDefaultAsync(rp
                => rp.RoleId == inputModel.RoleId && rp.PermissionId == inputModel.PermissionId);

            if (rolePermission == null)
            {
                return TypedResults.NotFound("Permission assignment not found");
            }

            dbContext.RolePermissions.Remove(rolePermission);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok();
        });
    }
}
