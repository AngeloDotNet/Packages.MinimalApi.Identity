using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;
using MinimalApi.Identity.API.Constants;
using MinimalApi.Identity.API.Enums;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Services.Interfaces;
using MinimalApi.Identity.API.Validations.Extensions;
using MinimalApi.Identity.Common.Extensions.Interfaces;

namespace MinimalApi.Identity.API.Endpoints;

public class ProfilesEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var apiGroup = endpoints
            .MapGroup(EndpointsApi.EndpointsProfilesGroup)
            .RequireAuthorization()
            .WithOpenApi(opt =>
            {
                opt.Tags = [new OpenApiTag { Name = EndpointsApi.EndpointsProfilesTag }];
                return opt;
            });

        apiGroup.MapGet(EndpointsApi.EndpointsProfile, async Task<IResult> ([FromServices] IProfileService profileService,
            [FromRoute] string username) =>
        {
            return await profileService.GetProfileAsync(username);
        })
        .Produces<UserProfileModel>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(nameof(Permissions.ProfiloRead))
        .WithOpenApi(opt =>
        {
            opt.Summary = "Get user profile";
            opt.Description = "Get user profile by username";
            return opt;
        });

        apiGroup.MapPut(EndpointsApi.EndpointsProfile, async Task<IResult> ([FromServices] IProfileService profileService,
            [FromRoute] string username, [FromBody] UserProfileEditModel inputModel) =>
        {
            return await profileService.EditProfileAsync(username, inputModel);
        })
        .Produces<string>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(nameof(Permissions.ProfiloWrite))
        .WithValidation<UserProfileEditModel>()
        .WithOpenApi(opt =>
        {
            opt.Summary = "Update user profile";
            opt.Description = "Update user profile by username";
            return opt;
        });

        //apiGroup.MapDelete(EndpointsApi.EndpointsProfile, async Task<IResult> ([FromServices] IProfileService profileService,
        //    [FromRoute] string username) =>
        //{
        //    return await profileService.DeleteProfileAsync(username);
        //})
        //.Produces<string>(StatusCodes.Status200OK)
        //.ProducesProblem(StatusCodes.Status401Unauthorized)
        //.ProducesProblem(StatusCodes.Status400BadRequest)
        //.ProducesProblem(StatusCodes.Status404NotFound)
        //.RequireAuthorization(nameof(Permissions.ProfiloWrite))
        //.WithOpenApi(opt =>
        //{
        //    opt.Summary = "Delete user profile";
        //    opt.Description = "Delete user profile by username";
        //    return opt;
        //});
    }
}
