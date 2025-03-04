using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.OpenApi.Models;
using MinimalApi.Identity.API.Constants;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.Common.Extensions.Interfaces;

namespace MinimalApi.Identity.API.Endpoints;

public class AccountEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var apiGroup = endpoints
            .MapGroup(EndpointsApi.EndpointsAccountGroup)
            .RequireAuthorization()
            .WithOpenApi(opt =>
            {
                opt.Tags = [new OpenApiTag { Name = EndpointsApi.EndpointsAccountTag }];
                return opt;
            });

        apiGroup.MapGet(EndpointsApi.EndpointsConfirmEmail, [AllowAnonymous] async Task<Results<Ok<string>,
            BadRequest<string>>> ([FromServices] UserManager<ApplicationUser> userManager, [FromRoute] string userId,
            [FromRoute] string token) =>
        {
            if (userId == null || token == null)
            {
                return TypedResults.BadRequest(MessageApi.UserIdTokenRequired);
            }

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return TypedResults.BadRequest(MessageApi.UserNotFound);
            }

            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(userId));
            var result = await userManager.ConfirmEmailAsync(user, code);

            if (!result.Succeeded)
            {
                return TypedResults.BadRequest(MessageApi.ErrorConfirmEmail);
            }

            return TypedResults.Ok(MessageApi.ConfirmingEmail);
        });
    }
}