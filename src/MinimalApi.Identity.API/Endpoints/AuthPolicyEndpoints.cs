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

public class AuthPolicyEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var apiGroup = endpoints
            .MapGroup(EndpointsApi.EndpointsAuthPolicyGroup)
            .RequireAuthorization()
            .WithOpenApi(opt =>
            {
                opt.Tags = [new OpenApiTag { Name = EndpointsApi.EndpointsAuthPolicyTag }];
                return opt;
            });

        apiGroup.MapGet(EndpointsApi.EndpointsStringEmpty, async ([FromServices] IAuthPolicyService authPolicyService) =>
        {
            return await authPolicyService.GetAllPoliciesAsync();
        })
        .Produces<Ok<List<PolicyResponseModel>>>(StatusCodes.Status200OK)
        .ProducesDefaultProblem(StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound)
        .RequireAuthorization(nameof(Permissions.AuthPolicyRead))
        .WithOpenApi(opt =>
        {
            opt.Description = "Get all authorization policy";
            opt.Summary = "Get all authorization policy";

            opt.Response(StatusCodes.Status200OK).Description = "Claims retrieved successfully";
            opt.Response(StatusCodes.Status401Unauthorized).Description = "Unauthorized";
            opt.Response(StatusCodes.Status404NotFound).Description = "Not found";

            return opt;
        });

        apiGroup.MapPost(EndpointsApi.EndpointsCreateAuthPolicy, async ([FromServices] IAuthPolicyService authPolicyService,
            [FromBody] CreatePolicyModel inputModel) =>
        {
            return await authPolicyService.CreatePolicyAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        .ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status401Unauthorized, StatusCodes.Status422UnprocessableEntity)
        .RequireAuthorization(nameof(Permissions.AuthPolicyWrite))
        .WithValidation<CreatePolicyModel>()
        .WithOpenApi(opt =>
        {
            opt.Description = "Add authorization policy";
            opt.Summary = "Add authorization policy";

            opt.Response(StatusCodes.Status200OK).Description = "Policy added successfully";
            opt.Response(StatusCodes.Status400BadRequest).Description = "Bad Request";
            opt.Response(StatusCodes.Status401Unauthorized).Description = "Unauthorized";

            return opt;
        });

        apiGroup.MapDelete(EndpointsApi.EndpointsDeleteAuthPolicy, async ([FromServices] IAuthPolicyService authPolicyService,
            [FromBody] DeletePolicyModel inputModel) =>
        {
            return await authPolicyService.DeletePolicyAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        .ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status401Unauthorized, StatusCodes.Status422UnprocessableEntity)
        .RequireAuthorization(nameof(Permissions.AuthPolicyWrite))
        .WithValidation<DeletePolicyModel>()
        .WithOpenApi(opt =>
        {
            opt.Description = "Delete authorization policy";
            opt.Summary = "Delete authorization policy";

            opt.Response(StatusCodes.Status200OK).Description = "Policy deleted successfully";
            opt.Response(StatusCodes.Status400BadRequest).Description = "Bad Request";
            opt.Response(StatusCodes.Status401Unauthorized).Description = "Unauthorized";

            return opt;
        });
    }
}
