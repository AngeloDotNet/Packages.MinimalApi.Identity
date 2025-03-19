using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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

        apiGroup.MapGet(EndpointsApi.EndpointsGetProfile, async Task<IResult> ([FromServices] IProfileService profileService,
            [FromRoute] int userId) =>
        {
            return await profileService.GetProfileAsync(userId);
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

        //apiGroup.MapPost(EndpointsApi.EndpointsCreateProfile, async Task<IResult> ([FromServices] IProfileService profileService,
        //    [FromBody] CreateUserProfileModel inputModel) =>
        //{
        //    return await profileService.CreateProfileAsync(inputModel);
        //})
        //.Produces<string>(StatusCodes.Status200OK)
        //.ProducesProblem(StatusCodes.Status401Unauthorized)
        //.ProducesProblem(StatusCodes.Status400BadRequest)
        //.ProducesProblem(StatusCodes.Status404NotFound)
        //.RequireAuthorization(nameof(Permissions.ProfiloWrite))
        //.WithValidation<CreateUserProfileModel>()
        //.WithOpenApi(opt =>
        //{
        //    opt.Summary = "Create user profile";
        //    opt.Description = "Create user profile by username";
        //    return opt;
        //});

        apiGroup.MapPut(EndpointsApi.EndpointsEditProfile, async Task<IResult> ([FromServices] IProfileService profileService,
            [FromBody] EditUserProfileModel inputModel) =>
        {
            return await profileService.EditProfileAsync(inputModel);
        })
        .Produces<string>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(nameof(Permissions.ProfiloWrite))
        .WithValidation<EditUserProfileModel>()
        .WithOpenApi(opt =>
        {
            opt.Summary = "Update user profile";
            opt.Description = "Update user profile by username";
            return opt;
        });

        //apiGroup.MapDelete(EndpointsApi.EndpointsDeleteProfile, async Task<IResult> ([FromServices] IProfileService profileService,
        //    [FromBody] DeleteUserProfileModel inputModel) =>
        //{
        //    return await profileService.DeleteProfileAsync(inputModel);
        //})
        //.Produces<string>(StatusCodes.Status200OK)
        //.ProducesProblem(StatusCodes.Status401Unauthorized)
        //.ProducesProblem(StatusCodes.Status400BadRequest)
        //.ProducesProblem(StatusCodes.Status404NotFound)
        //.RequireAuthorization(nameof(Permissions.ProfiloWrite))
        //.WithValidation<DeleteUserProfileModel>()
        //.WithOpenApi(opt =>
        //{
        //    opt.Summary = "Delete user profile";
        //    opt.Description = "Delete user profile by username";
        //    return opt;
        //});
    }
}
