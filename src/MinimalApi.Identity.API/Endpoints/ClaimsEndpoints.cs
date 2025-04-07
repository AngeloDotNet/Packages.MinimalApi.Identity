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

public class ClaimsEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var apiGroup = endpoints
            .MapGroup(EndpointsApi.EndpointsClaimsGroup)
            .RequireAuthorization()
            .WithOpenApi(opt =>
            {
                opt.Tags = [new OpenApiTag { Name = EndpointsApi.EndpointsClaimsTag }];
                return opt;
            });

        apiGroup.MapGet(EndpointsApi.EndpointsStringEmpty, async ([FromServices] IClaimsService claimsService) =>
        {
            return await claimsService.GetAllClaimsAsync();
        })
        .Produces<Ok<List<ClaimResponseModel>>>(StatusCodes.Status200OK)
        .ProducesDefaultProblem(StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound)
        .RequireAuthorization(nameof(Permissions.ClaimRead))
        .WithOpenApi(opt =>
        {
            opt.Description = "Get all claims";
            opt.Summary = "Get all claims";

            opt.Response(StatusCodes.Status200OK).Description = "Claims retrieved successfully";
            opt.Response(StatusCodes.Status401Unauthorized).Description = "Unauthorized";
            opt.Response(StatusCodes.Status404NotFound).Description = "Not found";

            return opt;
        });

        apiGroup.MapPost(EndpointsApi.EndpointsCreateClaim, async ([FromServices] IClaimsService claimsService,
            [FromBody] CreateClaimModel inputModel) =>
        {
            return await claimsService.CreateClaimAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        .ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status401Unauthorized, StatusCodes.Status422UnprocessableEntity)
        .RequireAuthorization(nameof(Permissions.ClaimWrite))
        .WithValidation<CreateClaimModel>()
        .WithOpenApi(opt =>
        {
            opt.Description = "Add claim";
            opt.Summary = "Add claim";

            opt.Response(StatusCodes.Status200OK).Description = "Claim added successfully";
            opt.Response(StatusCodes.Status400BadRequest).Description = "Bad Request";
            opt.Response(StatusCodes.Status401Unauthorized).Description = "Unauthorized";

            return opt;
        });

        apiGroup.MapPost(EndpointsApi.EndpointsAssignClaim, async ([FromServices] IClaimsService claimsService,
            [FromBody] AssignClaimModel inputModel) =>
        {
            return await claimsService.AssignClaimAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        .ProducesDefaultProblem(StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound, StatusCodes.Status422UnprocessableEntity)
        .RequireAuthorization(nameof(Permissions.ClaimWrite))
        .WithValidation<AssignClaimModel>()
        .WithOpenApi(opt =>
        {
            opt.Description = "Assign a claim to a user";
            opt.Summary = "Assign a claim to a user";

            opt.Response(StatusCodes.Status200OK).Description = "Claim assigned successfully";
            opt.Response(StatusCodes.Status401Unauthorized).Description = "Unauthorized";
            opt.Response(StatusCodes.Status404NotFound).Description = "Not found";

            return opt;
        });

        apiGroup.MapDelete(EndpointsApi.EndpointsRevokeClaim, async ([FromServices] IClaimsService claimService,
            [FromBody] RevokeClaimModel inputModel) =>
        {
            return await claimService.RevokeClaimAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        .ProducesDefaultProblem(StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound, StatusCodes.Status422UnprocessableEntity)
        .RequireAuthorization(nameof(Permissions.ClaimWrite))
        .WithValidation<RevokeClaimModel>()
        .WithOpenApi(opt =>
        {
            opt.Description = "Revoke a claim from a user";
            opt.Summary = "Revoke a claim from a user";

            opt.Response(StatusCodes.Status200OK).Description = "Claim revoked successfully";
            opt.Response(StatusCodes.Status401Unauthorized).Description = "Unauthorized";
            opt.Response(StatusCodes.Status404NotFound).Description = "Not found";

            return opt;
        });

        apiGroup.MapDelete(EndpointsApi.EndpointsDeleteClaim, async ([FromServices] IClaimsService claimsService,
            [FromBody] DeleteClaimModel inputModel) =>
        {
            return await claimsService.DeleteClaimAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        .ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status401Unauthorized, StatusCodes.Status422UnprocessableEntity)
        .RequireAuthorization(nameof(Permissions.ClaimWrite))
        .WithValidation<DeleteClaimModel>()
        .WithOpenApi(opt =>
        {
            opt.Description = "Delete claim";
            opt.Summary = "Delete claim";

            opt.Response(StatusCodes.Status200OK).Description = "Claim deleted successfully";
            opt.Response(StatusCodes.Status400BadRequest).Description = "Bad Request";
            opt.Response(StatusCodes.Status401Unauthorized).Description = "Unauthorized";

            return opt;
        });
    }
}
