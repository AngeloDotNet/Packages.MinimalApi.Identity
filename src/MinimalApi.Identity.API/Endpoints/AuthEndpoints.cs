using Microsoft.AspNetCore.Authorization;
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

public class AuthEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var apiGroup = endpoints
            .MapGroup(EndpointsApi.EndpointsAuthGroup)
            .RequireAuthorization()
            .WithOpenApi(opt =>
            {
                opt.Tags = [new OpenApiTag { Name = EndpointsApi.EndpointsAuthTag }];
                return opt;
            });

        apiGroup.MapPost(EndpointsApi.EndpointsAuthRegister, [AllowAnonymous] async ([FromServices] IAuthService authService,
            [FromBody] RegisterModel inputModel) =>
        {
            return await authService.RegisterAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        .ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status422UnprocessableEntity)
        .WithValidation<RegisterModel>()
        .WithOpenApi(opt =>
        {
            opt.Description = "Register new user";
            opt.Summary = "Register new user";

            opt.Response(StatusCodes.Status200OK).Description = "User registered successfully";
            opt.Response(StatusCodes.Status400BadRequest).Description = "Bad Request";

            return opt;
        });

        apiGroup.MapPost(EndpointsApi.EndpointsAuthLogin, [AllowAnonymous] async ([FromServices] IAuthService authService,
            [FromBody] LoginModel inputModel) =>
        {
            return await authService.LoginAsync(inputModel);
        })
        .Produces<Ok<AuthResponseModel>>(StatusCodes.Status200OK)
        .ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status422UnprocessableEntity)
        .WithValidation<LoginModel>()
        .WithOpenApi(opt =>
        {
            opt.Description = "Login user";
            opt.Summary = "Login user";

            opt.Response(StatusCodes.Status200OK).Description = "User logged in successfully";
            opt.Response(StatusCodes.Status400BadRequest).Description = "Bad Request";
            return opt;
        });

        apiGroup.MapPost(EndpointsApi.EndpointsAuthRefreshToken, [AllowAnonymous] async ([FromServices] IAuthService authService,
            [FromBody] RefreshTokenModel inputModel) =>
        {
            return await authService.RefreshTokenAsync(inputModel);
        })
        .Produces<Ok<AuthResponseModel>>(StatusCodes.Status200OK)
        .ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status422UnprocessableEntity)
        .WithValidation<RefreshTokenModel>()
        .WithOpenApi(opt =>
        {
            opt.Description = "Refresh token user";
            opt.Summary = "Refresh token user";

            opt.Response(StatusCodes.Status200OK).Description = "Token refreshed successfully";
            opt.Response(StatusCodes.Status400BadRequest).Description = "Bad Request";
            return opt;
        });

        apiGroup.MapPost(EndpointsApi.EndpointsImpersonateUser, async ([FromServices] IAuthService authService,
            [FromBody] ImpersonateUserModel inputModel) =>
        {
            return await authService.ImpersonateAsync(inputModel);
        })
        .Produces<Ok<AuthResponseModel>>(StatusCodes.Status200OK)
        .ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status422UnprocessableEntity)
        .WithValidation<ImpersonateUserModel>()
        .WithOpenApi(opt =>
        {
            opt.Description = "Impersonate user";
            opt.Summary = "Impersonate user";

            opt.Response(StatusCodes.Status200OK).Description = "User impersonated successfully";
            opt.Response(StatusCodes.Status400BadRequest).Description = "Bad Request";
            return opt;
        });

        apiGroup.MapPost(EndpointsApi.EndpointsAuthLogout, [AllowAnonymous] async ([FromServices] IAuthService authService) =>
        {
            return await authService.LogoutAsync();
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        .WithOpenApi(opt =>
        {
            opt.Description = "Logout user";
            opt.Summary = "Logout user";

            opt.Response(StatusCodes.Status200OK).Description = "User logged out successfully";

            return opt;
        });

        apiGroup.MapPost(EndpointsApi.EndpointsForgotPassword, async ([FromServices] IAuthService authService,
            [FromBody] ForgotPasswordModel inputModel) =>
        {
            return await authService.ForgotPasswordAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        .ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status422UnprocessableEntity)
        .WithValidation<ForgotPasswordModel>()
        .WithOpenApi(opt =>
        {
            opt.Description = "Forgot password";
            opt.Summary = "Forgot password";

            opt.Response(StatusCodes.Status200OK).Description = "Password reset link sent successfully";
            opt.Response(StatusCodes.Status400BadRequest).Description = "Bad Request";
            return opt;
        });
    }
}