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

public class RolesEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var apiGroup = endpoints
            .MapGroup(EndpointsApi.EndpointsRolesGroup)
            .RequireAuthorization()
            .WithOpenApi(opt =>
            {
                opt.Tags = [new OpenApiTag { Name = EndpointsApi.EndpointsRolesTag }];
                return opt;
            });

        apiGroup.MapGet(EndpointsApi.EndpointsStringEmpty, async Task<Results<Ok<List<RoleResponseModel>>, NotFound<string>>>
            ([FromServices] RoleManager<ApplicationRole> roleManager) =>
        {
            var query = await roleManager.Roles.ToListAsync();
            var result = query.Select(r => new RoleResponseModel(r.Id, r.Name!, r.Default)).ToList();

            return result == null ? TypedResults.NotFound(MessageApi.RolesNotFound) : TypedResults.Ok(result);
        })
        .Produces<List<RoleResponseModel>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(nameof(Authorize.GetRoles))
        .WithOpenApi(opt =>
        {
            opt.Summary = "Get all roles";
            opt.Description = "Get all roles";
            return opt;
        });

        apiGroup.MapPost(EndpointsApi.EndpointsCreateRole, async Task<Results<Ok<string>, BadRequest<IEnumerable<IdentityError>>,
            Conflict<string>>> ([FromServices] RoleManager<ApplicationRole> roleManager, [FromBody] string roleName) =>
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                return TypedResults.Conflict(MessageApi.RoleExists);
            }

            var result = await roleManager.CreateAsync(new ApplicationRole(roleName));

            return result.Succeeded ? TypedResults.Ok(MessageApi.RoleCreated) : TypedResults.BadRequest(result.Errors);
        })
        .Produces<string>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .RequireAuthorization(nameof(Authorize.CreateRole))
        .WithOpenApi(opt =>
        {
            opt.Summary = "Create role";
            opt.Description = "Create role";
            return opt;
        });

        apiGroup.MapPost(EndpointsApi.EndpointsAssignRole, async Task<Results<Ok<string>, NotFound<string>,
            BadRequest<IEnumerable<IdentityError>>>> ([FromServices] UserManager<ApplicationUser> userManager,
            [FromServices] RoleManager<ApplicationRole> roleManager, [FromBody] AssignRoleModel inputModel) =>
        {
            var user = await userManager.FindByNameAsync(inputModel.Username);

            if (user == null)
            {
                return TypedResults.NotFound(MessageApi.UserNotFound);
            }

            var roleExists = await roleManager.RoleExistsAsync(inputModel.Role);

            if (!roleExists)
            {
                return TypedResults.NotFound(MessageApi.RoleNotFound);
            }

            var result = await userManager.AddToRoleAsync(user, inputModel.Role);

            return result.Succeeded ? TypedResults.Ok(MessageApi.RoleAssigned) : TypedResults.BadRequest(result.Errors);
        })
        .Produces<string>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(nameof(Authorize.AssignRole))
        .WithOpenApi(opt =>
        {
            opt.Summary = "Assign role";
            opt.Description = "Assign role to user";
            return opt;
        });

        apiGroup.MapDelete(EndpointsApi.EndpointsRevokeRole, async Task<Results<Ok<string>, NotFound<string>,
            BadRequest<IEnumerable<IdentityError>>>> ([FromServices] UserManager<ApplicationUser> userManager,
            [FromServices] RoleManager<ApplicationRole> roleManager, [FromBody] RevokeRoleModel inputModel) =>
        {
            var user = await userManager.FindByNameAsync(inputModel.Username);

            if (user == null)
            {
                return TypedResults.NotFound(MessageApi.UserNotFound);
            }

            var result = await userManager.RemoveFromRoleAsync(user, inputModel.Role);

            return result.Succeeded ? TypedResults.Ok(MessageApi.RoleCanceled) : TypedResults.BadRequest(result.Errors);
        })
        .Produces<string>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(nameof(Authorize.DeleteRole))
        .WithOpenApi(opt =>
        {
            opt.Summary = "Revoke role";
            opt.Description = "Revoke role from user";
            return opt;
        });
    }
}
