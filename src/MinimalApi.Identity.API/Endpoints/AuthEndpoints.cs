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

        apiGroup.MapPost(EndpointsApi.EndpointsAuthRegister, [AllowAnonymous] async Task<IResult> ([FromServices] IAuthService authService,
            [FromBody] RegisterModel inputModel) =>
        {
            return await authService.RegisterAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithValidation<RegisterModel>()
        .WithOpenApi(opt =>
        {
            opt.Description = "Register new user";
            opt.Summary = "Register new user";

            return opt;
        });

        apiGroup.MapPost(EndpointsApi.EndpointsAuthLogin, [AllowAnonymous] async Task<IResult> ([FromServices] IAuthService authService,
            [FromBody] LoginModel inputModel) =>
        {
            return await authService.LoginAsync(inputModel);
        })
        .Produces<Ok<AuthResponseModel>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
        .WithValidation<LoginModel>()
        .WithOpenApi(opt =>
        {
            opt.Description = "Login user";
            opt.Summary = "Login user";

            return opt;
        });

        apiGroup.MapPost(EndpointsApi.EndpointsAuthLogout, [AllowAnonymous] async Task<IResult> ([FromServices] IAuthService authService) =>
        {
            return await authService.LogoutAsync();
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        .WithOpenApi(opt =>
        {
            opt.Description = "Logout user";
            opt.Summary = "Logout user";

            return opt;
        });

        //apiGroup.MapPost(EndpointsApi.EndpointsForgotPassword, async Task<Results<Ok<string>, NotFound<string>>>
        //        ([FromServices] UserManager<ApplicationUser> userManager, [FromServices] IEmailSender emailSender,
        //        [FromServices] IHttpContextAccessor httpContextAccessor, [FromBody] ForgotPasswordModel inputModel) =>
        //    {
        //        var user = await userManager.FindByEmailAsync(inputModel.Email);

        //        if (user == null)
        //        {
        //            return TypedResults.NotFound(MessageApi.UserNotFound);
        //        }

        //        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        //        var request = httpContextAccessor.HttpContext!.Request;

        //        await emailSender.SendEmailAsync(user.Email!, "Reset Password", $"To reset your password, you will need to indicate " +
        //            $"this token: {token}. It is recommended to copy and paste for simplicity.");

        //        return TypedResults.Ok(MessageApi.SendEmailResetPassword);
        //    })
        //    .WithOpenApi();

        //apiGroup.MapPost(EndpointsApi.EndpointsResetPassword, async Task<Results<Ok<string>, NotFound<string>,
        //    BadRequest<IEnumerable<IdentityError>>>> ([FromServices] UserManager<ApplicationUser> userManager,
        //    [FromBody] ResetPasswordModel inputModel) =>
        //{
        //    var user = await userManager.FindByEmailAsync(inputModel.Email);
        //    if (user == null)
        //    {
        //        return TypedResults.NotFound(MessageApi.UserNotFound);
        //    }

        //    var result = await userManager.ResetPasswordAsync(user, inputModel.Token, inputModel.Password);

        //    if (result.Succeeded)
        //    {
        //        return TypedResults.Ok(MessageApi.ResetPassword);
        //    }

        //    return TypedResults.BadRequest(result.Errors);
        //})
        //.WithOpenApi();
    }
}