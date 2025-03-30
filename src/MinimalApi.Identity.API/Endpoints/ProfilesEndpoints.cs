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

        apiGroup.MapGet(EndpointsApi.EndpointsStringEmpty, async Task<IResult> ([FromServices] IProfileService profileService) =>
        {
            return TypedResults.Ok(await profileService.GetProfilesAsync());
        })
        .Produces<List<UserProfileModel>>(StatusCodes.Status200OK)
        .ProducesDefaultProblem(StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound)
        .RequireAuthorization(nameof(Permissions.ProfiloRead))
        .WithOpenApi(opt =>
        {
            opt.Summary = "Get all profiles";
            opt.Description = "Get all profiles";

            opt.Response(StatusCodes.Status200OK).Description = "List of users profiles";
            opt.Response(StatusCodes.Status401Unauthorized).Description = "Unauthorized";
            opt.Response(StatusCodes.Status404NotFound).Description = "Not found";

            return opt;
        });

        apiGroup.MapGet(EndpointsApi.EndpointsGetProfile, async Task<IResult> ([FromServices] IProfileService profileService,
            [FromRoute] int userId) =>
        {
            return TypedResults.Ok(await profileService.GetProfileAsync(userId));
        })
        .Produces<UserProfileModel>(StatusCodes.Status200OK)
        .ProducesDefaultProblem(StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound)
        .RequireAuthorization(nameof(Permissions.ProfiloRead))
        .WithOpenApi(opt =>
        {
            opt.Summary = "Get user profile";
            opt.Description = "Get user profile by username";

            opt.Response(StatusCodes.Status200OK).Description = "User profile retrieved successfully";
            opt.Response(StatusCodes.Status401Unauthorized).Description = "Unauthorized";
            opt.Response(StatusCodes.Status404NotFound).Description = "Not found";
            return opt;
        });

        apiGroup.MapPost(EndpointsApi.EndpointsChangeEnableProfile, async Task<IResult> ([FromServices] IProfileService profileService,
            [FromBody] ChangeEnableProfileModel inputModel) =>
        {
            return TypedResults.Ok(await profileService.ChangeEnablementStatusUserProfileAsync(inputModel));
        })
        .Produces<UserProfileModel>(StatusCodes.Status200OK)
        .ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound)
        .RequireAuthorization(nameof(Permissions.ProfiloRead))
        .WithOpenApi(opt =>
        {
            opt.Summary = "Edit user profile enablement";
            opt.Description = "Edit user profile enablement";

            opt.Response(StatusCodes.Status200OK).Description = "User profile retrieved successfully";
            opt.Response(StatusCodes.Status400BadRequest).Description = "Bad request";
            opt.Response(StatusCodes.Status401Unauthorized).Description = "Unauthorized";
            opt.Response(StatusCodes.Status404NotFound).Description = "Not found";
            return opt;
        });

        apiGroup.MapPut(EndpointsApi.EndpointsEditProfile, async Task<IResult> ([FromServices] IProfileService profileService,
            [FromBody] EditUserProfileModel inputModel) =>
        {
            return TypedResults.Ok(await profileService.EditProfileAsync(inputModel));
        })
        .Produces<string>(StatusCodes.Status200OK)
        .ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound, StatusCodes.Status422UnprocessableEntity)
        .RequireAuthorization(nameof(Permissions.ProfiloWrite))
        .WithValidation<EditUserProfileModel>()
        .WithOpenApi(opt =>
        {
            opt.Summary = "Update user profile";
            opt.Description = "Update user profile by username";

            opt.Response(StatusCodes.Status200OK).Description = "User profile updated successfully";
            opt.Response(StatusCodes.Status400BadRequest).Description = "Bad request";
            opt.Response(StatusCodes.Status401Unauthorized).Description = "Unauthorized";
            opt.Response(StatusCodes.Status404NotFound).Description = "Not found";

            return opt;
        });
    }
}
