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

public class LicenzeEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var apiGroup = endpoints
            .MapGroup(EndpointsApi.EndpointsLicenzeGroup)
            .RequireAuthorization()
            .WithOpenApi(opt =>
            {
                opt.Tags = [new OpenApiTag { Name = EndpointsApi.EndpointsLicenzeTag }];
                return opt;
            });

        apiGroup.MapGet(EndpointsApi.EndpointsStringEmpty, async ([FromServices] ILicenseService licenseService) =>
        {
            return await licenseService.GetAllLicensesAsync();
        })
        .Produces<Ok<List<LicenseResponseModel>>>(StatusCodes.Status200OK)
        .ProducesDefaultProblem(StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound)
        .RequireAuthorization(nameof(Permissions.LicenzaRead))
        .WithOpenApi(opt =>
        {
            opt.Description = "Get all licenses";
            opt.Summary = "Get all licenses";

            opt.Response(StatusCodes.Status200OK).Description = "Licenses retrieved successfully";
            opt.Response(StatusCodes.Status401Unauthorized).Description = "Unauthorized";
            opt.Response(StatusCodes.Status404NotFound).Description = "Not found";

            return opt;
        });

        apiGroup.MapPost(EndpointsApi.EndpointsCreateLicense, async ([FromServices] ILicenseService licenseService,
            [FromBody] CreateLicenseModel inputModel) =>
        {
            return await licenseService.CreateLicenseAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        .ProducesDefaultProblem(StatusCodes.Status401Unauthorized, StatusCodes.Status422UnprocessableEntity)
        .RequireAuthorization(nameof(Permissions.LicenzaWrite))
        .WithValidation<CreateLicenseModel>()
        .WithOpenApi(opt =>
        {
            opt.Description = "Create a new license";
            opt.Summary = "Create a new license";

            opt.Response(StatusCodes.Status200OK).Description = "License created successfully";
            opt.Response(StatusCodes.Status401Unauthorized).Description = "Unauthorized";

            return opt;
        });

        apiGroup.MapPost(EndpointsApi.EndpointsAssignLicense, async ([FromServices] ILicenseService licenseService,
            [FromBody] AssignLicenseModel inputModel) =>
        {
            return await licenseService.AssignLicenseAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        .ProducesDefaultProblem(StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound, StatusCodes.Status422UnprocessableEntity)
        .RequireAuthorization(nameof(Permissions.LicenzaWrite))
        .WithValidation<AssignLicenseModel>()
        .WithOpenApi(opt =>
        {
            opt.Description = "Assign a license to a user";
            opt.Summary = "Assign a license to a user";

            opt.Response(StatusCodes.Status200OK).Description = "License assigned successfully";
            opt.Response(StatusCodes.Status401Unauthorized).Description = "Unauthorized";
            opt.Response(StatusCodes.Status404NotFound).Description = "Not found";

            return opt;
        });

        apiGroup.MapDelete(EndpointsApi.EndpointsRevokeLicense, async ([FromServices] ILicenseService licenseService,
            [FromBody] RevokeLicenseModel inputModel) =>
        {
            return await licenseService.RevokeLicenseAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        .ProducesDefaultProblem(StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound, StatusCodes.Status422UnprocessableEntity)
        .RequireAuthorization(nameof(Permissions.LicenzaWrite))
        .WithValidation<RevokeLicenseModel>()
        .WithOpenApi(opt =>
        {
            opt.Description = "Revoke a license from a user";
            opt.Summary = "Revoke a license from a user";

            opt.Response(StatusCodes.Status200OK).Description = "License revoked successfully";
            opt.Response(StatusCodes.Status401Unauthorized).Description = "Unauthorized";
            opt.Response(StatusCodes.Status404NotFound).Description = "Not found";

            return opt;
        });

        apiGroup.MapDelete(EndpointsApi.EndpointsDeleteLicense, async ([FromServices] ILicenseService licenseService,
            [FromBody] DeleteLicenseModel inputModel) =>
        {
            return await licenseService.DeleteLicenseAsync(inputModel);
        })
        .Produces<string>(StatusCodes.Status200OK)
        .ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound, StatusCodes.Status422UnprocessableEntity)
        .RequireAuthorization(nameof(Permissions.LicenzaWrite))
        .WithValidation<DeleteLicenseModel>()
        .WithOpenApi(opt =>
        {
            opt.Summary = "Delete license";
            opt.Description = "Delete license";

            opt.Response(StatusCodes.Status200OK).Description = "License deleted successfully";
            opt.Response(StatusCodes.Status400BadRequest).Description = "Bad Request";
            opt.Response(StatusCodes.Status401Unauthorized).Description = "Unauthorized";
            opt.Response(StatusCodes.Status404NotFound).Description = "Not found";

            return opt;
        });
    }
}