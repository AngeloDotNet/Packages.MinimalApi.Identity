using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalApi.Identity.API.Constants;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Extensions;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Options;
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

        apiGroup.MapPost(EndpointsApi.EndpointsAuthRegister, [AllowAnonymous] async Task<Results<Ok<string>,
            BadRequest<IEnumerable<IdentityError>>>> ([FromServices] UserManager<ApplicationUser> userManager,
            [FromBody] RegisterModel inputModel) =>
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
                return TypedResults.Ok(MessageApi.UserCreated);
            }

            return TypedResults.BadRequest(result.Errors);
        })
        .WithOpenApi();

        apiGroup.MapPost(EndpointsApi.EndpointsAuthLogin, [AllowAnonymous] async Task<Results<Ok<AuthResponse>, BadRequest<string>>>
            ([FromServices] IConfiguration configuration, [FromServices] UserManager<ApplicationUser> userManager,
            [FromServices] IAuthService authService, [FromBody] LoginModel inputModel) =>
        {
            var user = await userManager.FindByNameAsync(inputModel.Username);

            if (user == null || !await userManager.CheckPasswordAsync(user, inputModel.Password))
            {
                return TypedResults.BadRequest(MessageApi.InvalidCredentials);
            }

            await userManager.UpdateSecurityStampAsync(user);

            var userRoles = await userManager.GetRolesAsync(user);
            var rolePermissions = await authService.GetPermissionsFromRolesAsync(userRoles);

            var claims = new List<Claim>()
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, inputModel.Username),
                new(ClaimTypes.Email, user.Email ?? string.Empty),
                new(ClaimTypes.SerialNumber, user.SecurityStamp!.ToString()),
            }
            .Union(rolePermissions.Select(permission => new Claim(ClaimsExtensions.Permission, permission)))
            .Union(userRoles.Select(role => new Claim(ClaimTypes.Role, role))).ToList();

            var loginResponse = CreateToken(claims, configuration);

            //user.RefreshToken = loginResponse.RefreshToken;
            //user.RefreshTokenExpirationDate = DateTime.UtcNow.AddMinutes(60);

            await userManager.UpdateAsync(user);

            return TypedResults.Ok(loginResponse);
        })
        .WithOpenApi();

        apiGroup.MapPost(EndpointsApi.EndpointsForgotPassword, async Task<Results<Ok<string>, NotFound<string>>>
                ([FromServices] UserManager<ApplicationUser> userManager, [FromServices] IEmailSender emailSender,
                [FromServices] IHttpContextAccessor httpContextAccessor, [FromBody] ForgotPasswordModel inputModel) =>
            {
                var user = await userManager.FindByEmailAsync(inputModel.Email);

                if (user == null)
                {
                    return TypedResults.NotFound(MessageApi.UserNotFound);
                }

                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var request = httpContextAccessor.HttpContext!.Request;

                await emailSender.SendEmailAsync(user.Email!, "Reset Password", $"To reset your password, you will need to indicate " +
                    $"this token: {token}. It is recommended to copy and paste for simplicity.");

                return TypedResults.Ok(MessageApi.SendEmailResetPassword);
            })
            .WithOpenApi();

        apiGroup.MapPost(EndpointsApi.EndpointsResetPassword, async Task<Results<Ok<string>, NotFound<string>,
            BadRequest<IEnumerable<IdentityError>>>> ([FromServices] UserManager<ApplicationUser> userManager,
            [FromBody] ResetPasswordModel inputModel) =>
        {
            var user = await userManager.FindByEmailAsync(inputModel.Email);
            if (user == null)
            {
                return TypedResults.NotFound(MessageApi.UserNotFound);
            }

            var result = await userManager.ResetPasswordAsync(user, inputModel.Token, inputModel.Password);

            if (result.Succeeded)
            {
                return TypedResults.Ok(MessageApi.ResetPassword);
            }

            return TypedResults.BadRequest(result.Errors);
        })
        .WithOpenApi();
    }

    private static AuthResponse CreateToken(IList<Claim> claims, IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSettingsOptions<JwtOptions>(nameof(JwtOptions));

        var audienceClaim = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Aud);
        claims.Remove(audienceClaim!);

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecurityKey));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(jwtOptions.Issuer, jwtOptions.Audience, claims,
            DateTime.UtcNow, DateTime.UtcNow.AddMinutes(60), signingCredentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        var italyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
        var expiredLocalNow = TimeZoneInfo.ConvertTimeFromUtc(jwtSecurityToken.ValidTo, italyTimeZone);

        var response = new AuthResponse(accessToken, GenerateRefreshToken(), expiredLocalNow);
        return response;

        static string GenerateRefreshToken()
        {
            var randomNumber = new byte[256];
            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }
    }
}