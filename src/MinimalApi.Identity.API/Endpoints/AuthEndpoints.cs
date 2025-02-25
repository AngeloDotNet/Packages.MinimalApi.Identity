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
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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
            ([FromServices] UserManager<ApplicationUser> userManager, [FromBody] LoginModel inputModel) =>
        {
            var user = await userManager.FindByNameAsync(inputModel.Username);

            if (user == null || !await userManager.CheckPasswordAsync(user, inputModel.Password))
            {
                return TypedResults.Unauthorized();
            }

            await userManager.UpdateSecurityStampAsync(user);

            var userRoles = await userManager.GetRolesAsync(user);
            var claims = new List<Claim>()
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, inputModel.Username),
                new(ClaimTypes.Email, user.Email ?? string.Empty),
                new(ClaimTypes.SerialNumber, user.SecurityStamp!.ToString()),
            }.Union(userRoles.Select(role => new Claim(ClaimTypes.Role, role))).ToList();

            var loginResponse = await CreateTokenAsync(claims);

            //user.RefreshToken = loginResponse.RefreshToken;
            //user.RefreshTokenExpirationDate = DateTime.UtcNow.AddMinutes(60);

            await userManager.UpdateAsync(user);

            return TypedResults.Ok(loginResponse);
        });

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
            });

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
        });
    }

    private static async Task<AuthResponse> CreateTokenAsync(IList<Claim> claims)
    {
        var audienceClaim = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Aud);
        claims.Remove(audienceClaim!);

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("aY4Rz54vETFOlfl8gZJ4p95d2cWVMAC7HZMnqtggJfjeQATIvVroknVUKcP0ClSE9r9u856UjGKcXkyr7ugB0X3FsxWvbnDcaqIjhYZAwtt0CU24RLgCdoIjQl5tnapPjNgZYLsHl7qUMLASbNSL5MkTbfCjhBOjqcrkbBeMoHZfdwkQrmslBq6xgtcLOUlLA3GmSvhq15eAPaq5kX0J85rtfTVpGr1ZShJLUVVaASm50thUXJEuxeQLKB2a5bcLC4OuX9xkjS7LqpKpCAwTEEOXcrYJAKJ03Tf7tRRnplknkP6QPOQqPviKQXRZ47gwR0sTZwL9ZHpRokEyaTpRLkyNLpSnRAvwwk2N8tezsZPWjd3rCecC38QM9RKF1xCWtxooo8qR1kTGXdGzORRX1RCpR717koB1xFdPJbLYV1c3za1vWpf7foYyfGHdj9XfvnEogNgygsJnSBAa9ghv3mQzbXspOQafIweknF2PCd2FBlSSTdt8JY2iSDGbD2gy"));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken("iss-localhost", "aud-localhost", claims,
            DateTime.UtcNow, DateTime.UtcNow.AddMinutes(60), signingCredentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        var italyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
        var expiredLocalNow = TimeZoneInfo.ConvertTimeFromUtc(jwtSecurityToken.ValidTo, italyTimeZone);

        var response = new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = GenerateRefreshToken(),
            ExpiredToken = expiredLocalNow
        };

        return response;

        static string GenerateRefreshToken()
        {
            var randomNumber = new byte[256];
            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }
    }

    private class AuthResponse
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public DateTime ExpiredToken { get; set; }
    }
}
