using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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

public class ModuliEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var apiGroup = endpoints
            .MapGroup(EndpointsApi.EndpointsModulesGroup)
            .RequireAuthorization()
            .WithOpenApi(opt =>
            {
                opt.Tags = [new OpenApiTag { Name = EndpointsApi.EndpointsModulesTag }];
                return opt;
            });

        apiGroup.MapGet(EndpointsApi.EndpointsStringEmpty, async Task<IResult> ([FromServices] IModuleService moduleService) =>
        {
            return await moduleService.GetAllModulesAsync();
        })
        .Produces<Ok<List<ModuleResponseModel>>>(StatusCodes.Status200OK)
        .ProducesDefaultProblem(StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound)
        .RequireAuthorization(nameof(Permissions.ModuloRead))
        .WithOpenApi(opt =>
        {
            opt.Description = "Get all modules";
            opt.Summary = "Get all modules";

            opt.Response(StatusCodes.Status200OK).Description = "Modules retrieved successfully";
            opt.Response(StatusCodes.Status401Unauthorized).Description = "Unauthorized";
            opt.Response(StatusCodes.Status404NotFound).Description = "Not found";

            return opt;
        });

        apiGroup.MapPost(EndpointsApi.EndpointsCreateModule, async Task<IResult> ([FromServices] IModuleService moduleService,
            [FromBody] CreateModuleModel inputModel) =>
        {
            return await moduleService.CreateModuleAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        .ProducesDefaultProblem(StatusCodes.Status401Unauthorized, StatusCodes.Status422UnprocessableEntity)
        .RequireAuthorization(nameof(Permissions.ModuloWrite))
        .WithValidation<CreateModuleModel>()
        .WithOpenApi(opt =>
        {
            opt.Description = "Create a new module";
            opt.Summary = "Create a new module";

            opt.Response(StatusCodes.Status200OK).Description = "Module created successfully";
            opt.Response(StatusCodes.Status401Unauthorized).Description = "Unauthorized";

            return opt;
        });

        apiGroup.MapPost(EndpointsApi.EndpointsAssignModule, async Task<IResult> ([FromServices] IModuleService moduleService,
            [FromBody] AssignModuleModel inputModel) =>
        {
            return await moduleService.AssignModuleAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        .ProducesDefaultProblem(StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound, StatusCodes.Status422UnprocessableEntity)
        .RequireAuthorization(nameof(Permissions.ModuloWrite))
        .WithValidation<AssignModuleModel>()
        .WithOpenApi(opt =>
        {
            opt.Description = "Assign a module to a user";
            opt.Summary = "Assign a module to a user";

            opt.Response(StatusCodes.Status200OK).Description = "Module assigned successfully";
            opt.Response(StatusCodes.Status401Unauthorized).Description = "Unauthorized";
            opt.Response(StatusCodes.Status404NotFound).Description = "Not found";

            return opt;
        });

        apiGroup.MapDelete(EndpointsApi.EndpointsRevokeModule, async Task<IResult> ([FromServices] IModuleService moduleService,
            [FromBody] RevokeModuleModel inputModel) =>
        {
            return await moduleService.RevokeModuleAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        .ProducesDefaultProblem(StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound, StatusCodes.Status422UnprocessableEntity)
        .RequireAuthorization(nameof(Permissions.ModuloWrite))
        .WithValidation<RevokeModuleModel>()
        .WithOpenApi(opt =>
        {
            opt.Description = "Revoke a module from a user";
            opt.Summary = "Revoke a module from a user";

            opt.Response(StatusCodes.Status200OK).Description = "Module revoked successfully";
            opt.Response(StatusCodes.Status401Unauthorized).Description = "Unauthorized";
            opt.Response(StatusCodes.Status404NotFound).Description = "Not found";

            return opt;
        });

        apiGroup.MapDelete(EndpointsApi.EndpointsDeleteModule, async Task<IResult> ([FromServices] IModuleService moduleService,
            [FromBody] DeleteModuleModel inputModel) =>
        {
            return await moduleService.DeleteModuleAsync(inputModel);
        })
        .Produces<string>(StatusCodes.Status200OK)
        .ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound, StatusCodes.Status422UnprocessableEntity)
        .RequireAuthorization(nameof(Permissions.ModuloWrite))
        .WithValidation<DeleteModuleModel>()
        .WithOpenApi(opt =>
        {
            opt.Summary = "Delete module";
            opt.Description = "Delete module";

            opt.Response(StatusCodes.Status200OK).Description = "Module deleted successfully";
            opt.Response(StatusCodes.Status400BadRequest).Description = "Bad request";
            opt.Response(StatusCodes.Status401Unauthorized).Description = "Unauthorized";
            opt.Response(StatusCodes.Status404NotFound).Description = "Not found";

            return opt;
        });
    }
}
