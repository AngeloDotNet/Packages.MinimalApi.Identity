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

        apiGroup.MapGet(EndpointsApi.EndpointsStringEmpty, async Task<Results<Ok<List<Module>>, NotFound<string>>>
            (MinimalApiDbContext dbContext) =>
        {
            var result = await dbContext.Modules.ToListAsync();

            if (result == null)
            {
                return TypedResults.NotFound(MessageApi.ModulesNotFound);
            }

            return TypedResults.Ok(result);
        })
        .RequireAuthorization(nameof(Authorization.GetModules))
        .WithOpenApi();

        apiGroup.MapPost(EndpointsApi.EndpointsCreateModule, async (MinimalApiDbContext dbContext, [FromBody] Module inputModel) =>
        {
            dbContext.Modules.Add(inputModel);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok(MessageApi.ModuleCreated);
        })
        .RequireAuthorization(nameof(Authorization.CreateModule))
        .WithOpenApi();

        apiGroup.MapPost(EndpointsApi.EndpointsAssignModule, async Task<Results<Ok<string>, NotFound<string>>> (MinimalApiDbContext dbContext,
           [FromServices] UserManager<ApplicationUser> userManager, [FromBody] AssignModuleModel inputModel) =>
        {
            var user = await userManager.FindByIdAsync(inputModel.UserId.ToString());

            if (user == null)
            {
                return TypedResults.NotFound(MessageApi.UserNotFound);
            }

            var module = await dbContext.Modules.FindAsync(inputModel.ModuleId);
            if (module == null)
            {
                return TypedResults.NotFound(MessageApi.ModuleNotFound);
            }

            var userModule = new UserModule
            {
                UserId = inputModel.UserId,
                ModuleId = inputModel.ModuleId
            };

            dbContext.UserModules.Add(userModule);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok(MessageApi.ModuleAssigned);
        })
        .RequireAuthorization(nameof(Authorization.AssignModule))
        .WithOpenApi();

        apiGroup.MapDelete(EndpointsApi.EndpointsRevokeModule, async Task<Results<Ok<string>, NotFound<string>>>
            (MinimalApiDbContext dbContext, [FromBody] AssignModuleModel inputModel) =>
        {
            var userModule = await dbContext.UserModules
                .SingleOrDefaultAsync(um => um.UserId == inputModel.UserId && um.ModuleId == inputModel.ModuleId);

            if (userModule == null)
            {
                return TypedResults.NotFound(MessageApi.ModuleNotFound);
            }

            dbContext.UserModules.Remove(userModule);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok(MessageApi.ModuleCanceled);
        })
        .RequireAuthorization(nameof(Authorization.DeleteModule))
        .WithOpenApi();
    }
}
