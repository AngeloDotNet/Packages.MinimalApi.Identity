using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MinimalApi.Identity.Common.Extensions.Interfaces;
using MinimalApi.Identity.DataAccessLayer;
using MinimalApi.Identity.DataAccessLayer.Entities;
using MinimalApi.Identity.Shared;

namespace MinimalApi.Identity.API.Endpoints;

public class LicenzeEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var apiGroup = endpoints
            .MapGroup("/licenze")
            .RequireAuthorization()
            .WithOpenApi(opt =>
            {
                opt.Tags = [new OpenApiTag { Name = "Licenze" }];
                return opt;
            });

        apiGroup.MapGet(string.Empty, async Task<Results<Ok<List<License>>, BadRequest>> (MinimalApiDbContext dbContext) =>
        {
            var result = await dbContext.Licenses.ToListAsync();

            if (result == null)
            {
                return TypedResults.BadRequest();
            }

            return TypedResults.Ok(result);
        })
        .WithOpenApi();

        apiGroup.MapPost("/crea-licenza", async (MinimalApiDbContext dbContext, [FromBody] License inputModel) =>
        {
            dbContext.Licenses.Add(inputModel);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok();
        })
        .WithOpenApi();

        apiGroup.MapPost("/assegna-licenza", async Task<Results<Ok, NotFound<string>>> (MinimalApiDbContext dbContext,
            [FromServices] UserManager<ApplicationUser> userManager, [FromBody] AssignLicenseModel inputModel) =>
        {
            var user = await userManager.FindByIdAsync(inputModel.UserId.ToString());

            if (user == null)
            {
                return TypedResults.NotFound("User not found");
            }

            var license = await dbContext.Licenses.FindAsync(inputModel.LicenseId);
            if (license == null)
            {
                return TypedResults.NotFound("License not found");
            }

            var userLicense = new UserLicense
            {
                UserId = inputModel.UserId,
                LicenseId = inputModel.LicenseId
            };

            dbContext.UserLicenses.Add(userLicense);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok();
        })
        .WithOpenApi();

        apiGroup.MapDelete("/rimuovi-licenza", async Task<Results<Ok, NotFound<string>>> (MinimalApiDbContext dbContext,
             [FromBody] AssignLicenseModel inputModel) =>
        {
            var userLicense = await dbContext.UserLicenses.SingleOrDefaultAsync(ul
                => ul.UserId == inputModel.UserId && ul.LicenseId == inputModel.LicenseId);

            if (userLicense == null)
            {
                return TypedResults.NotFound("License assignment not found");
            }

            dbContext.UserLicenses.Remove(userLicense);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok();
        })
        .WithOpenApi();
    }
}