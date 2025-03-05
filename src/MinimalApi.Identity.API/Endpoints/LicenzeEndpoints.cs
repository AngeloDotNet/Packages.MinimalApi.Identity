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

        apiGroup.MapGet(EndpointsApi.EndpointsStringEmpty, async Task<Results<Ok<List<License>>, NotFound<string>>>
            (MinimalApiDbContext dbContext) =>
        {
            var result = await dbContext.Licenses.ToListAsync();

            //if (result == null)
            //{
            //    return TypedResults.NotFound(MessageApi.LicensesNotFound);
            //}

            //return TypedResults.Ok(result);

            return result == null ? TypedResults.NotFound(MessageApi.LicensesNotFound) : TypedResults.Ok(result);
        })
        .Produces<Ok<List<License>>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(nameof(Authorization.GetLicenses))
        .WithOpenApi(opt =>
        {
            opt.Description = "Get all licenses";
            opt.Summary = "Get all licenses";
            return opt;
        });

        apiGroup.MapPost(EndpointsApi.EndpointsCreateLicense, async (MinimalApiDbContext dbContext, [FromBody] License inputModel) =>
        {
            dbContext.Licenses.Add(inputModel);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok(MessageApi.LicenseCreated);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(nameof(Authorization.CreateLicense))
        .WithOpenApi(opt =>
        {
            opt.Description = "Create a new license";
            opt.Summary = "Create a new license";
            return opt;
        });

        apiGroup.MapPost(EndpointsApi.EndpointsAssignLicense, async Task<Results<Ok<string>, NotFound<string>>>
            (MinimalApiDbContext dbContext, [FromServices] UserManager<ApplicationUser> userManager, [FromBody] AssignLicenseModel inputModel) =>
        {
            var user = await userManager.FindByIdAsync(inputModel.UserId.ToString());

            if (user == null)
            {
                return TypedResults.NotFound(MessageApi.UserNotFound);
            }

            var license = await dbContext.Licenses.FindAsync(inputModel.LicenseId);

            if (license == null)
            {
                return TypedResults.NotFound(MessageApi.LicenseNotFound);
            }

            var userLicense = new UserLicense
            {
                UserId = inputModel.UserId,
                LicenseId = inputModel.LicenseId
            };

            dbContext.UserLicenses.Add(userLicense);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok(MessageApi.LicenseAssigned);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(nameof(Authorization.AssignLicense))
        .WithOpenApi(opt =>
        {
            opt.Description = "Assign a license to a user";
            opt.Summary = "Assign a license to a user";
            return opt;
        });

        apiGroup.MapDelete(EndpointsApi.EndpointsRevokeLicense, async Task<Results<Ok<string>, NotFound<string>>>
            (MinimalApiDbContext dbContext, [FromBody] AssignLicenseModel inputModel) =>
        {
            var userLicense = await dbContext.UserLicenses.SingleOrDefaultAsync(ul
                => ul.UserId == inputModel.UserId && ul.LicenseId == inputModel.LicenseId);

            if (userLicense == null)
            {
                return TypedResults.NotFound(MessageApi.LicenseNotFound);
            }

            dbContext.UserLicenses.Remove(userLicense);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok(MessageApi.LicenseCanceled);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(nameof(Authorization.DeleteLicense))
        .WithOpenApi(opt =>
        {
            opt.Description = "Revoke a license from a user";
            opt.Summary = "Revoke a license from a user";
            return opt;
        });
    }
}