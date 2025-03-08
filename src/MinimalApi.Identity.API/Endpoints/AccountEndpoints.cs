using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;
using MinimalApi.Identity.API.Constants;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Services.Interfaces;
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

        apiGroup.MapGet(EndpointsApi.EndpointsConfirmEmail, [AllowAnonymous] async Task<IResult>
            ([FromServices] IAccountService accountService, [FromRoute] string userId, [FromRoute] string token) =>
        {
            return await accountService.ConfirmEmailAsync(userId, token);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithOpenApi(opt =>
        {
            opt.Description = "Confirm email address";
            opt.Summary = "Confirm email address";

            return opt;
        });

        apiGroup.MapPost(EndpointsApi.EndpointChangeEmail, async Task<IResult> ([FromServices] IAccountService accountService,
            ChangeEmailModel inputModel) =>
        {
            return await accountService.ChangeEmailAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithOpenApi(opt =>
        {
            opt.Description = "Change email address";
            opt.Summary = "Change email address";

            return opt;
        });

        apiGroup.MapGet(EndpointsApi.EndpointsConfirmEmailChange, [AllowAnonymous] async Task<IResult>
            ([FromServices] IAccountService accountService, [FromRoute] string userId, [FromRoute] string email,
            [FromRoute] string token) =>
        {
            return await accountService.ConfirmEmailChangeAsync(userId, email, token);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithOpenApi();
    }
}