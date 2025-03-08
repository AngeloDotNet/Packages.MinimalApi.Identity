using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MinimalApi.Identity.API.Constants;
using MinimalApi.Identity.API.Database;
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

        apiGroup.MapGet(EndpointsApi.EndpointsStringEmpty, async Task<Results<Ok<List<PermissionResponseModel>>, NotFound<string>>>
            (MinimalApiDbContext dbContext) =>
        {
            var query = await dbContext.Permissions.ToListAsync();
            var result = query.Select(p => new PermissionResponseModel(p.Id, p.Name, p.Default)).ToList();

            return result == null ? TypedResults.NotFound(MessageApi.PermissionsNotFound) : TypedResults.Ok(result);
        })
        .Produces<Ok<List<PermissionResponseModel>>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(nameof(Authorize.GetPermissions))
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
        .RequireAuthorization(nameof(Authorize.CreatePermission))
        .WithOpenApi(opt =>
        {
            opt.Summary = "Create a new permission";
            opt.Description = "Create a new permission";
            return opt;
        });

        apiGroup.MapPost(EndpointsApi.EndpointsAssignPermission, async Task<Results<Ok<string>, NotFound<string>>>
            (MinimalApiDbContext dbContext, [FromServices] UserManager<ApplicationUser> userManager, [FromBody] AssignPermissionModel inputModel) =>
        {
            var user = await userManager.FindByIdAsync(inputModel.UserId.ToString());

            if (user == null)
            {
                return TypedResults.NotFound(MessageApi.UserNotFound);
            }

            var permission = await dbContext.Permissions.FindAsync(inputModel.PermissionId);

            if (permission == null)
            {
                return TypedResults.NotFound(MessageApi.PermissionNotFound);
            }

            var userPermission = new UserPermission
            {
                UserId = inputModel.UserId,
                PermissionId = inputModel.PermissionId
            };

            dbContext.UserPermissions.Add(userPermission);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok(MessageApi.PermissionAssigned);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(nameof(Authorize.AssignPermission))
        .WithOpenApi(opt =>
        {
            opt.Summary = "Assign a permission to a user";
            opt.Description = "Assign a permission to a user";
            return opt;
        });

        apiGroup.MapDelete(EndpointsApi.EndpointsRevokePermission, async Task<Results<Ok<string>, NotFound<string>>>
            (MinimalApiDbContext dbContext, [FromServices] UserManager<ApplicationUser> userManager, [FromBody] RevokePermissionModel inputModel) =>
        {
            var userPermission = await dbContext.UserPermissions.SingleOrDefaultAsync(up
                => up.UserId == inputModel.UserId && up.PermissionId == inputModel.PermissionId);

            if (userPermission == null)
            {
                return TypedResults.NotFound(MessageApi.PermissionNotFound);
            }

            dbContext.UserPermissions.Remove(userPermission);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok(MessageApi.PermissionCanceled);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(nameof(Authorize.DeletePermission))
        .WithOpenApi(opt =>
        {
            opt.Summary = "Revoke a permission from a user";
            opt.Description = "Revoke a permission from a user";
            return opt;
        });
    }
}