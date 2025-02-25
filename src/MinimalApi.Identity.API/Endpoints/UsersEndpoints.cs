using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;
using MinimalApi.Identity.Common.Extensions.Interfaces;
using MinimalApi.Identity.DataAccessLayer.Entities;
using MinimalApi.Identity.Shared;

namespace MinimalApi.Identity.API.Endpoints;

public class UsersEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var apiGroup = endpoints
            .MapGroup("/utenti")
            .RequireAuthorization()
            .WithOpenApi(opt =>
            {
                opt.Tags = [new OpenApiTag { Name = "Utenti" }];
                return opt;
            });

        apiGroup.MapPost("/registrazione", [AllowAnonymous] async Task<Results<Ok<string>, BadRequest<IEnumerable<IdentityError>>>>
            ([FromServices] UserManager<ApplicationUser> userManager, [FromBody] RegisterModel inputModel) =>
        {
            var user = new ApplicationUser
            {
                FirstName = inputModel.FirstName,
                LastName = inputModel.LastName,
                UserName = inputModel.Username,
                Email = inputModel.Email
            };

            var result = await userManager.CreateAsync(user, inputModel.Password);

            if (result.Succeeded)
            {
                return TypedResults.Ok("User registered successfully");
            }

            return TypedResults.BadRequest(result.Errors);
        });

        apiGroup.MapGet("/profilo/{username}", [Authorize("Users")] async Task<Results<Ok<UserProfileModel>,
            NotFound<string>>> ([FromServices] UserManager<ApplicationUser> userManager, [FromRoute] string username) =>
        {
            var user = await userManager.FindByNameAsync(username);

            if (user == null)
            {
                return TypedResults.NotFound("User not found");
            }

            var profile = new UserProfileModel
            {
                Username = user.UserName!,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };

            return TypedResults.Ok(profile);
        });

        apiGroup.MapPut("/profilo/{username}", async Task<Results<Ok<string>, NotFound<string>,
            BadRequest<IEnumerable<IdentityError>>>> ([FromServices] UserManager<ApplicationUser> userManager,
            [FromRoute] string username, [FromBody] UserProfileModel inputModel) =>
        {
            var user = await userManager.FindByNameAsync(username);

            if (user == null)
            {
                return TypedResults.NotFound("User not found");
            }

            user.FirstName = inputModel.FirstName;
            user.LastName = inputModel.LastName;
            user.Email = inputModel.Email;

            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return TypedResults.Ok("Profile updated successfully");
            }

            return TypedResults.BadRequest(result.Errors);
        });

        apiGroup.MapDelete("/profilo/{username}", async Task<Results<Ok<string>, NotFound<string>,
            BadRequest<IEnumerable<IdentityError>>>> ([FromServices] UserManager<ApplicationUser> userManager,
            [FromRoute] string username) =>
        {
            var user = await userManager.FindByNameAsync(username);

            if (user == null)
            {
                return TypedResults.NotFound("User not found");
            }

            var result = await userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return TypedResults.Ok("User deleted successfully");
            }

            return TypedResults.BadRequest(result.Errors);
        });
    }
}
