using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;
using MinimalApi.Identity.API.Constants;
using MinimalApi.Identity.API.Enums;
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
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(nameof(Authorize.GetModules))
        .WithOpenApi(opt =>
        {
            opt.Description = "Get all modules";
            opt.Summary = "Get all modules";
            return opt;
        });

        apiGroup.MapPost(EndpointsApi.EndpointsCreateModule, async Task<IResult> ([FromServices] IModuleService moduleService,
            [FromBody] CreateModuleModel inputModel) =>
        {
            return await moduleService.CreateModuleAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(nameof(Authorize.CreateModule))
        .WithOpenApi(opt =>
        {
            opt.Description = "Create a new module";
            opt.Summary = "Create a new module";
            return opt;
        });

        apiGroup.MapPost(EndpointsApi.EndpointsAssignModule, async Task<IResult> ([FromServices] IModuleService moduleService,
            [FromBody] AssignModuleModel inputModel) =>
        {
            return await moduleService.AssignModuleAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(nameof(Authorize.AssignModule))
        .WithOpenApi(opt =>
        {
            opt.Description = "Assign a module to a user";
            opt.Summary = "Assign a module to a user";
            return opt;
        });

        apiGroup.MapDelete(EndpointsApi.EndpointsRevokeModule, async Task<IResult> ([FromServices] IModuleService moduleService,
            [FromBody] RevokeModuleModel inputModel) =>
        {
            return await moduleService.RevokeModuleAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(nameof(Authorize.DeleteModule))
        .WithOpenApi(opt =>
        {
            opt.Description = "Revoke a module from a user";
            opt.Summary = "Revoke a module from a user";
            return opt;
        });
    }
}
