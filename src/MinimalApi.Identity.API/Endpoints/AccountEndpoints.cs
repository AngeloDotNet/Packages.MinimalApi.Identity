﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;
using MinimalApi.Identity.API.Constants;
using MinimalApi.Identity.API.Extensions;
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
        //.ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesDefaultProblem(StatusCodes.Status400BadRequest)
        .WithOpenApi(opt =>
        {
            opt.Description = "Confirm email address";
            opt.Summary = "Confirm email address";

            opt.Response(StatusCodes.Status200OK).Description = "Email address confirmed successfully";
            opt.Response(StatusCodes.Status400BadRequest).Description = "Bad Request";

            return opt;
        });

        apiGroup.MapPost(EndpointsApi.EndpointChangeEmail, async Task<IResult> ([FromServices] IAccountService accountService,
            ChangeEmailModel inputModel) =>
        {
            return await accountService.ChangeEmailAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        //.ProducesProblem(StatusCodes.Status400BadRequest)
        //.ProducesProblem(StatusCodes.Status401Unauthorized)
        //.ProducesProblem(StatusCodes.Status422UnprocessableEntity)
        .ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status401Unauthorized, StatusCodes.Status422UnprocessableEntity)
        .WithValidation<ChangeEmailModel>()
        .WithOpenApi(opt =>
        {
            opt.Description = "Change email address";
            opt.Summary = "Change email address";

            opt.Response(StatusCodes.Status200OK).Description = "Email address changed successfully";
            opt.Response(StatusCodes.Status400BadRequest).Description = "Bad Request";
            opt.Response(StatusCodes.Status401Unauthorized).Description = "Unauthorized";

            return opt;
        });

        apiGroup.MapGet(EndpointsApi.EndpointsConfirmEmailChange, [AllowAnonymous] async Task<IResult>
            ([FromServices] IAccountService accountService, [FromRoute] string userId, [FromRoute] string email,
            [FromRoute] string token) =>
        {
            return await accountService.ConfirmEmailChangeAsync(userId, email, token);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        //.ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesDefaultProblem(StatusCodes.Status400BadRequest)
        .WithOpenApi(opt =>
        {
            opt.Description = "Confirm email address change";
            opt.Summary = "Confirm email address change";

            opt.Response(StatusCodes.Status200OK).Description = "Email address change confirmed successfully";
            opt.Response(StatusCodes.Status400BadRequest).Description = "Bad Request";

            return opt;
        });
    }
}