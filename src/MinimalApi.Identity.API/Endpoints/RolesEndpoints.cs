using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;
using MinimalApi.Identity.API.Constants;
using MinimalApi.Identity.API.Enums;
using MinimalApi.Identity.API.Extensions;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Services.Interfaces;
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

        apiGroup.MapGet(EndpointsApi.EndpointsStringEmpty, async Task<IResult> ([FromServices] IRoleService roleService) =>
        {
            return await roleService.GetAllRolesAsync();
        })
        .Produces<List<RoleResponseModel>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(nameof(Permissions.RuoloRead))
        .WithOpenApi(opt =>
        {
            opt.Summary = "Get all roles";
            opt.Description = "Get all roles";
            return opt;
        });

        apiGroup.MapPost(EndpointsApi.EndpointsCreateRole, async Task<IResult> ([FromServices] IRoleService roleService,
            [FromBody] CreateRoleModel inputModel) =>
        {
            return await roleService.CreateRoleAsync(inputModel);
        })
        .Produces<string>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .RequireAuthorization(nameof(Permissions.RuoloWrite))
        .WithValidation<CreateRoleModel>()
        .WithOpenApi(opt =>
        {
            opt.Summary = "Create role";
            opt.Description = "Create role";
            return opt;
        });

        apiGroup.MapPost(EndpointsApi.EndpointsAssignRole, async Task<IResult> ([FromServices] IRoleService roleService,
            [FromBody] AssignRoleModel inputModel) =>
        {
            return await roleService.AssignRoleAsync(inputModel);
        })
        .Produces<string>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(nameof(Permissions.RuoloWrite))
        .WithValidation<AssignRoleModel>()
        .WithOpenApi(opt =>
        {
            opt.Summary = "Assign role";
            opt.Description = "Assign role to user";
            return opt;
        });

        apiGroup.MapDelete(EndpointsApi.EndpointsRevokeRole, async Task<IResult> ([FromServices] IRoleService roleService,
            [FromBody] RevokeRoleModel inputModel) =>
        {
            return await roleService.RevokeRoleAsync(inputModel);
        })
        .Produces<string>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(nameof(Permissions.RuoloWrite))
        .WithValidation<RevokeRoleModel>()
        .WithOpenApi(opt =>
        {
            opt.Summary = "Revoke role";
            opt.Description = "Revoke role from user";
            return opt;
        });

        apiGroup.MapDelete(EndpointsApi.EndpointsDeleteRole, async Task<IResult> ([FromServices] IRoleService roleService,
            [FromBody] DeleteRoleModel inputModel) =>
        {
            return await roleService.DeleteRoleAsync(inputModel);
        })
        .Produces<string>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(nameof(Permissions.RuoloWrite))
        .WithValidation<DeleteRoleModel>()
        .WithOpenApi(opt =>
        {
            opt.Summary = "Delete role";
            opt.Description = "Delete role";
            return opt;
        });
    }
}
