using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
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
            .MapGroup("/ruoli")
            .RequireAuthorization()
            .WithOpenApi(opt =>
            {
                opt.Tags = [new OpenApiTag { Name = "Ruoli" }];
                return opt;
            });

        apiGroup.MapGet(string.Empty, async Task<Results<Ok<List<ApplicationRole>>, BadRequest>>
            ([FromServices] RoleManager<ApplicationRole> roleManager) =>
        {
            var result = await roleManager.Roles.ToListAsync();

            if (result == null)
            {
                return TypedResults.BadRequest();
            }

            return TypedResults.Ok(result);
        })
        .RequireAuthorization(nameof(Authorization.GetRoles))
        .WithOpenApi();

        apiGroup.MapPost("/crea-ruolo", async Task<Results<Ok, BadRequest<IEnumerable<IdentityError>>, Conflict<string>>>
            ([FromServices] RoleManager<ApplicationRole> roleManager, [FromBody] string roleName) =>
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                return TypedResults.Conflict("The role already exist");
            }

            var result = await roleManager.CreateAsync(new ApplicationRole(roleName));

            if (result.Succeeded)
            {
                return TypedResults.Ok();
            }

            return TypedResults.BadRequest(result.Errors);
        })
        .RequireAuthorization(nameof(Authorization.CreateRole))
        .WithOpenApi();

        apiGroup.MapPost("/assegna-ruolo", async Task<Results<Ok<string>, NotFound<string>, BadRequest<IEnumerable<IdentityError>>>>
            ([FromServices] UserManager<ApplicationUser> userManager, [FromServices] RoleManager<ApplicationRole> roleManager,
            [FromBody] AssignRoleModel inputModel) =>
        {
            var user = await userManager.FindByNameAsync(inputModel.Username);

            if (user == null)
            {
                return TypedResults.NotFound("User not found");
            }

            var roleExists = await roleManager.RoleExistsAsync(inputModel.Role);

            if (!roleExists)
            {
                return TypedResults.NotFound("Role not found");
            }

            var result = await userManager.AddToRoleAsync(user, inputModel.Role);

            if (result.Succeeded)
            {
                return TypedResults.Ok("Role assigned successfully");
            }

            return TypedResults.BadRequest(result.Errors);
        })
        .RequireAuthorization(nameof(Authorization.AssignRole))
        .WithOpenApi();

        apiGroup.MapDelete("/rimuovi-ruolo", async Task<Results<Ok, NotFound<string>, BadRequest<IEnumerable<IdentityError>>>>
            ([FromServices] UserManager<ApplicationUser> userManager, [FromServices] RoleManager<ApplicationRole> roleManager,
            [FromBody] AssignRoleModel inputModel) =>
        {
            var user = await userManager.FindByNameAsync(inputModel.Username);

            if (user == null)
            {
                return TypedResults.NotFound("User not found");
            }

            var result = await userManager.RemoveFromRoleAsync(user, inputModel.Role);

            if (result.Succeeded)
            {
                return TypedResults.Ok();
            }

            return TypedResults.BadRequest(result.Errors);
        })
        .RequireAuthorization(nameof(Authorization.DeleteRole))
        .WithOpenApi();
    }
}
