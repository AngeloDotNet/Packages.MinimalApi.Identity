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

            if (result == null)
            {
                return TypedResults.NotFound(MessageApi.LicensesNotFound);
            }

            return TypedResults.Ok(result);
        })
        .RequireAuthorization(nameof(Authorization.GetLicenses))
        .WithOpenApi();

        apiGroup.MapPost(EndpointsApi.EndpointsCreateLicense, async (MinimalApiDbContext dbContext, [FromBody] License inputModel) =>
        {
            dbContext.Licenses.Add(inputModel);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok(MessageApi.LicenseCreated);
        })
        .RequireAuthorization(nameof(Authorization.CreateLicense))
        .WithOpenApi();

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
        .RequireAuthorization(nameof(Authorization.AssignLicense))
        .WithOpenApi();

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
        .RequireAuthorization(nameof(Authorization.DeleteLicense))
        .WithOpenApi();
    }
}