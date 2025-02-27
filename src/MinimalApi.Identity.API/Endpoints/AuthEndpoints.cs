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
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalApi.Identity.BusinessLayer.Extensions;
using MinimalApi.Identity.BusinessLayer.Options;
using MinimalApi.Identity.BusinessLayer.Services.Interfaces;
using MinimalApi.Identity.Common.Extensions.Interfaces;
using MinimalApi.Identity.DataAccessLayer.Entities;
using MinimalApi.Identity.Shared;

namespace MinimalApi.Identity.API.Endpoints;

public class AuthEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var apiGroup = endpoints
            .MapGroup("/autenticazione")
            .RequireAuthorization()
            .WithOpenApi(opt =>
            {
                opt.Tags = [new OpenApiTag { Name = "Autenticazione" }];
                return opt;
            });

        apiGroup.MapPost("/login", [AllowAnonymous] async Task<Results<Ok<AuthResponse>, UnauthorizedHttpResult>>
            ([FromServices] IConfiguration configuration, [FromServices] UserManager<ApplicationUser> userManager,
            [FromServices] IAuthService authService, [FromBody] LoginModel inputModel) =>
        {
            var user = await userManager.FindByNameAsync(inputModel.Username);

            if (user == null || !await userManager.CheckPasswordAsync(user, inputModel.Password))
            {
                return TypedResults.Unauthorized();
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

                //new(ClaimsExtensions.Permission, nameof(PermissionPolicy.GetPermission)),
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

        apiGroup.MapPost("/forgot-password", async Task<Results<Ok<string>, BadRequest<string>>>
                ([FromServices] UserManager<ApplicationUser> userManager, [FromServices] IEmailSender emailSender,
                [FromServices] IHttpContextAccessor httpContextAccessor, [FromBody] ForgotPasswordModel inputModel) =>
            {
                var user = await userManager.FindByEmailAsync(inputModel.Email);

                if (user == null)
                {
                    return TypedResults.BadRequest("User not found");
                }

                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var request = httpContextAccessor.HttpContext!.Request;

                var queryParams = new Dictionary<string, string?>
                {
                    { "token", token },
                    { "email", user.Email }
                };

                var callbackUrl = QueryHelpers.AddQueryString($"{request.Scheme}://{request.Host}/reset-password", queryParams);

                await emailSender.SendEmailAsync(user.Email!, "Reset Password", $"Please reset your password by <a href='{callbackUrl}'>clicking here</a>.");

                return TypedResults.Ok("Password reset email sent");
            })
            .WithOpenApi();

        apiGroup.MapPost("/reset-password", async Task<Results<Ok<string>, BadRequest<string>, BadRequest<IEnumerable<IdentityError>>>>
            ([FromServices] UserManager<ApplicationUser> userManager, [FromBody] ResetPasswordModel inputModel) =>
        {
            var user = await userManager.FindByEmailAsync(inputModel.Email);
            if (user == null)
            {
                return TypedResults.BadRequest("User not found");
            }

            var result = await userManager.ResetPasswordAsync(user, inputModel.Token, inputModel.Password);

            if (result.Succeeded)
            {
                return TypedResults.Ok("Password reset successful");
            }

            return TypedResults.BadRequest(result.Errors);
        })
        .WithOpenApi();
    }

    private static AuthResponse CreateToken(IList<Claim> claims, IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>()
            ?? throw new ArgumentNullException("JWT options not found");

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