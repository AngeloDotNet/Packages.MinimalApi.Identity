using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;
using MinimalApi.Identity.API.Constants;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Enums;
using MinimalApi.Identity.API.Models;
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

        apiGroup.MapGet(EndpointsApi.EndpointsProfile, async Task<Results<Ok<UserProfileModel>, NotFound<string>>>
            ([FromServices] UserManager<ApplicationUser> userManager, [FromRoute] string username) =>
        {
            var user = await userManager.FindByNameAsync(username);

            //if (user == null)
            //{
            //    return TypedResults.NotFound(MessageApi.ProfileNotFound);
            //}

            //return TypedResults.Ok(new UserProfileModel(username, user.Email!, user.FirstName, user.LastName));

            return user == null ? TypedResults.NotFound(MessageApi.ProfileNotFound) : TypedResults.Ok(new UserProfileModel(username, user.Email!, user.FirstName, user.LastName));
        })
        .Produces<UserProfileModel>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(nameof(Authorization.GetProfile))
        .WithOpenApi(opt =>
        {
            opt.Summary = "Get user profile";
            opt.Description = "Get user profile by username";
            return opt;
        });

        apiGroup.MapPut(EndpointsApi.EndpointsProfile, async Task<Results<Ok<string>, NotFound<string>,
            BadRequest<IEnumerable<IdentityError>>>> ([FromServices] UserManager<ApplicationUser> userManager,
            [FromRoute] string username, [FromBody] UserProfileModel inputModel) =>
        {
            var user = await userManager.FindByNameAsync(username);

            if (user == null)
            {
                return TypedResults.NotFound(MessageApi.ProfileNotFound);
            }

            user.FirstName = inputModel.FirstName;
            user.LastName = inputModel.LastName;
            user.Email = inputModel.Email;

            var result = await userManager.UpdateAsync(user);

            //if (result.Succeeded)
            //{
            //    return TypedResults.Ok(MessageApi.ProfileUpdated);
            //}

            //return TypedResults.BadRequest(result.Errors);

            return result.Succeeded ? TypedResults.Ok(MessageApi.ProfileUpdated) : TypedResults.BadRequest(result.Errors);
        })
        .Produces<string>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(nameof(Authorization.EditProfile))
        .WithOpenApi(opt =>
        {
            opt.Summary = "Update user profile";
            opt.Description = "Update user profile by username";
            return opt;
        });

        apiGroup.MapDelete(EndpointsApi.EndpointsProfile, async Task<Results<Ok<string>, NotFound<string>,
            BadRequest<IEnumerable<IdentityError>>>> ([FromServices] UserManager<ApplicationUser> userManager, [FromRoute] string username) =>
        {
            var user = await userManager.FindByNameAsync(username);

            if (user == null)
            {
                return TypedResults.NotFound(MessageApi.ProfileNotFound);
            }

            var result = await userManager.DeleteAsync(user);

            //if (result.Succeeded)
            //{
            //    return TypedResults.Ok(MessageApi.UserDeleted);
            //}

            //return TypedResults.BadRequest(result.Errors);

            return result.Succeeded ? TypedResults.Ok(MessageApi.UserDeleted) : TypedResults.BadRequest(result.Errors);
        })
        .Produces<string>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(nameof(Authorization.DeleteProfile))
        .WithOpenApi(opt =>
        {
            opt.Summary = "Delete user profile";
            opt.Description = "Delete user profile by username";
            return opt;
        });
    }
}
