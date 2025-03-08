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

        apiGroup.MapGet(EndpointsApi.EndpointsStringEmpty, async Task<IResult> ([FromServices] ILicenseService licenseService) =>
        {
            return await licenseService.GetAllLicensesAsync();
        })
        .Produces<Ok<List<LicenseResponseModel>>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(nameof(Authorize.GetLicenses))
        .WithOpenApi(opt =>
        {
            opt.Description = "Get all licenses";
            opt.Summary = "Get all licenses";
            return opt;
        });

        apiGroup.MapPost(EndpointsApi.EndpointsCreateLicense, async Task<IResult> ([FromServices] ILicenseService licenseService,
            [FromBody] CreateLicenseModel inputModel) =>
        {
            return await licenseService.CreateLicenseAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(nameof(Authorize.CreateLicense))
        .WithOpenApi(opt =>
        {
            opt.Description = "Create a new license";
            opt.Summary = "Create a new license";
            return opt;
        });

        apiGroup.MapPost(EndpointsApi.EndpointsAssignLicense, async Task<IResult> ([FromServices] ILicenseService licenseService,
            [FromBody] AssignLicenseModel inputModel) =>
        {
            return await licenseService.AssignLicenseAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(nameof(Authorize.AssignLicense))
        .WithOpenApi(opt =>
        {
            opt.Description = "Assign a license to a user";
            opt.Summary = "Assign a license to a user";
            return opt;
        });

        apiGroup.MapDelete(EndpointsApi.EndpointsRevokeLicense, async Task<IResult> ([FromServices] ILicenseService licenseService,
            [FromBody] RevokeLicenseModel inputModel) =>
        {
            return await licenseService.RevokeLicenseAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(nameof(Authorize.DeleteLicense))
        .WithOpenApi(opt =>
        {
            opt.Description = "Revoke a license from a user";
            opt.Summary = "Revoke a license from a user";
            return opt;
        });
    }
}