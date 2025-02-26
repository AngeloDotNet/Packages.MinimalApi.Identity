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

public class ModuliEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var apiGroup = endpoints
            .MapGroup("/moduli")
            .RequireAuthorization("Modules")
            .WithOpenApi(opt =>
            {
                opt.Tags = [new OpenApiTag { Name = "Moduli" }];
                return opt;
            });

        apiGroup.MapGet(string.Empty, async Task<Results<Ok<List<Module>>, BadRequest>> (MinimalApiDbContext dbContext) =>
        {
            var result = await dbContext.Modules.ToListAsync();

            if (result == null)
            {
                return TypedResults.BadRequest();
            }

            return TypedResults.Ok(result);
        })
        .WithOpenApi();

        apiGroup.MapPost("/crea-modulo", async (MinimalApiDbContext dbContext, [FromBody] Module inputModel) =>
        {
            dbContext.Modules.Add(inputModel);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok();
        })
        .WithOpenApi();

        apiGroup.MapPost("/assegna-modulo", async Task<Results<Ok, NotFound<string>>> (MinimalApiDbContext dbContext,
           [FromServices] UserManager<ApplicationUser> userManager, [FromBody] AssignModuleModel inputModel) =>
        {
            var user = await userManager.FindByIdAsync(inputModel.UserId.ToString());

            if (user == null)
            {
                return TypedResults.NotFound("User not found");
            }

            var module = await dbContext.Modules.FindAsync(inputModel.ModuleId);
            if (module == null)
            {
                return TypedResults.NotFound("Module not found");
            }

            var userModule = new UserModule
            {
                UserId = inputModel.UserId,
                ModuleId = inputModel.ModuleId
            };

            dbContext.UserModules.Add(userModule);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok();
        })
        .WithOpenApi();

        apiGroup.MapDelete("/rimuovi-modulo", async Task<Results<Ok, NotFound<string>>> (MinimalApiDbContext dbContext,
             [FromBody] AssignModuleModel inputModel) =>
        {
            var userModule = await dbContext.UserModules
                .SingleOrDefaultAsync(um => um.UserId == inputModel.UserId && um.ModuleId == inputModel.ModuleId);

            if (userModule == null)
            {
                return TypedResults.NotFound("Module assignment not found");
            }

            dbContext.UserModules.Remove(userModule);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok();
        })
        .WithOpenApi();
    }
}
