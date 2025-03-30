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
        .ProducesDefaultProblem(StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound)
        .RequireAuthorization(nameof(Permissions.RuoloRead))
        .WithOpenApi(opt =>
        {
            opt.Summary = "Get all roles";
            opt.Description = "Get all roles";

            opt.Response(StatusCodes.Status200OK).Description = "List of roles";
            opt.Response(StatusCodes.Status401Unauthorized).Description = "Unauthorized";
            opt.Response(StatusCodes.Status404NotFound).Description = "Not found";

            return opt;
        });

        apiGroup.MapPost(EndpointsApi.EndpointsCreateRole, async Task<IResult> ([FromServices] IRoleService roleService,
            [FromBody] CreateRoleModel inputModel) =>
        {
            return await roleService.CreateRoleAsync(inputModel);
        })
        .Produces<string>(StatusCodes.Status200OK)
        .ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status401Unauthorized, StatusCodes.Status409Conflict, StatusCodes.Status422UnprocessableEntity)
        .RequireAuthorization(nameof(Permissions.RuoloWrite))
        .WithValidation<CreateRoleModel>()
        .WithOpenApi(opt =>
        {
            opt.Summary = "Create role";
            opt.Description = "Create role";

            opt.Response(StatusCodes.Status200OK).Description = "Role created";
            opt.Response(StatusCodes.Status401Unauthorized).Description = "Unauthorized";
            opt.Response(StatusCodes.Status400BadRequest).Description = "Bad request";
            opt.Response(StatusCodes.Status409Conflict).Description = "Conflict";

            return opt;
        });

        apiGroup.MapPost(EndpointsApi.EndpointsAssignRole, async Task<IResult> ([FromServices] IRoleService roleService,
            [FromBody] AssignRoleModel inputModel) =>
        {
            return await roleService.AssignRoleAsync(inputModel);
        })
        .Produces<string>(StatusCodes.Status200OK)
        .ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound, StatusCodes.Status422UnprocessableEntity)
        .RequireAuthorization(nameof(Permissions.RuoloWrite))
        .WithValidation<AssignRoleModel>()
        .WithOpenApi(opt =>
        {
            opt.Summary = "Assign role";
            opt.Description = "Assign role to user";

            opt.Response(StatusCodes.Status200OK).Description = "Role assigned";
            opt.Response(StatusCodes.Status400BadRequest).Description = "Bad request";
            opt.Response(StatusCodes.Status401Unauthorized).Description = "Unauthorized";
            opt.Response(StatusCodes.Status404NotFound).Description = "Not found";

            return opt;
        });

        apiGroup.MapDelete(EndpointsApi.EndpointsRevokeRole, async Task<IResult> ([FromServices] IRoleService roleService,
            [FromBody] RevokeRoleModel inputModel) =>
        {
            return await roleService.RevokeRoleAsync(inputModel);
        })
        .Produces<string>(StatusCodes.Status200OK)
        .ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound, StatusCodes.Status422UnprocessableEntity)
        .RequireAuthorization(nameof(Permissions.RuoloWrite))
        .WithValidation<RevokeRoleModel>()
        .WithOpenApi(opt =>
        {
            opt.Summary = "Revoke role";
            opt.Description = "Revoke role from user";

            opt.Response(StatusCodes.Status200OK).Description = "Role revoked";
            opt.Response(StatusCodes.Status400BadRequest).Description = "Bad request";
            opt.Response(StatusCodes.Status401Unauthorized).Description = "Unauthorized";
            opt.Response(StatusCodes.Status404NotFound).Description = "Not found";

            return opt;
        });

        apiGroup.MapDelete(EndpointsApi.EndpointsDeleteRole, async Task<IResult> ([FromServices] IRoleService roleService,
            [FromBody] DeleteRoleModel inputModel) =>
        {
            return await roleService.DeleteRoleAsync(inputModel);
        })
        .Produces<string>(StatusCodes.Status200OK)
        .ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound, StatusCodes.Status422UnprocessableEntity)
        .RequireAuthorization(nameof(Permissions.RuoloWrite))
        .WithValidation<DeleteRoleModel>()
        .WithOpenApi(opt =>
        {
            opt.Summary = "Delete role";
            opt.Description = "Delete role";

            opt.Response(StatusCodes.Status200OK).Description = "Role deleted";
            opt.Response(StatusCodes.Status400BadRequest).Description = "Bad request";
            opt.Response(StatusCodes.Status401Unauthorized).Description = "Unauthorized";
            opt.Response(StatusCodes.Status404NotFound).Description = "Not found";

            return opt;
        });
    }
}
