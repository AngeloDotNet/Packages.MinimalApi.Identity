using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MinimalApi.Identity.API.Constants;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Enums;
using MinimalApi.Identity.API.Extensions;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Options;
using MinimalApi.Identity.API.Services.Interfaces;

namespace MinimalApi.Identity.API.Services;

public class AuthService(IConfiguration configuration, UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager, IEmailSenderService emailSender, IHttpContextAccessor httpContextAccessor) : IAuthService
{
    public async Task<IResult> LoginAsync(LoginModel model)
    {
        var identityOptions = configuration.GetSettingsOptions<NetIdentityOptions>(nameof(NetIdentityOptions));

        var signInResult = await signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe,
            identityOptions.AllowedForNewUsers);

        if (!signInResult.Succeeded)
        {
            return signInResult switch
            {
                { IsNotAllowed: true } => TypedResults.BadRequest(MessageApi.UserNotAllowedLogin),
                { IsLockedOut: true } => TypedResults.BadRequest(MessageApi.UserLockedOut),
                { RequiresTwoFactor: true } => TypedResults.BadRequest(MessageApi.RequiredTwoFactor),
                _ => TypedResults.BadRequest(MessageApi.InvalidCredentials)
            };
        }

        var user = await userManager.FindByNameAsync(model.Username);

        if (user == null)
        {
            return TypedResults.BadRequest(MessageApi.UserNotFound);
        }

        if (!user.EmailConfirmed)
        {
            return TypedResults.BadRequest(MessageApi.UserNotEmailConfirmed);
        }

        await userManager.UpdateSecurityStampAsync(user);

        var userRoles = await userManager.GetRolesAsync(user);
        var userClaims = await userManager.GetClaimsAsync(user);

        var claims = new List<Claim>()
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.GivenName, user.FirstName ?? string.Empty),
            new(ClaimTypes.Surname, user.LastName ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.SerialNumber, user.SecurityStamp!.ToString()),
        }
        .Union(userClaims)
        .Union(userRoles.Select(role => new Claim(ClaimTypes.Role, role))).ToList();

        if (user.FirstName != null && user.LastName != null)
        {
            claims.Add(new Claim(CustomClaimTypes.FullName, $"{user.FirstName} {user.LastName}"));
        }

        var loginResponse = CreateToken(claims, configuration);

        //user.RefreshToken = loginResponse.RefreshToken;
        //user.RefreshTokenExpirationDate = DateTime.UtcNow.AddMinutes(60);

        await userManager.UpdateAsync(user);

        return TypedResults.Ok(loginResponse);
    }

    private static AuthResponseModel CreateToken(IList<Claim> claims, IConfiguration configuration)
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

        var response = new AuthResponseModel(accessToken, GenerateRefreshToken(), expiredLocalNow);
        return response;

        static string GenerateRefreshToken()
        {
            var randomNumber = new byte[256];
            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }
    }

    public async Task<IResult> RegisterAsync(RegisterModel model)
    {
        var userOptions = configuration.GetSettingsOptions<UsersOptions>(nameof(UsersOptions));
        var user = new ApplicationUser
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            UserName = model.Username,
            Email = model.Email
        };

        var result = await userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            if (user.Email.Equals(userOptions.AssignAdminRoleOnRegistration, StringComparison.OrdinalIgnoreCase))
            {
                var roleAssignResult = await userManager.AddToRoleAsync(user, nameof(DefaultRoles.Admin));

                if (!roleAssignResult.Succeeded)
                {
                    return TypedResults.BadRequest(MessageApi.RoleNotAssigned);
                }

                var claimsAssingResult = await AddClaimsToAdminUserAsync(user);

                if (!claimsAssingResult.Succeeded)
                {
                    return TypedResults.BadRequest(MessageApi.ClaimsNotAssigned);
                }
            }

            var userId = await userManager.GetUserIdAsync(user);
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var callbackUrl = await GenerateCallBackUrlAsync(userId, token);

            await emailSender.SendEmailTypeAsync(user.Email, callbackUrl, 1);

            return TypedResults.Ok(MessageApi.UserCreated);
        }

        return TypedResults.BadRequest(result.Errors);
    }

    public async Task<IResult> LogoutAsync()
    {
        await signInManager.SignOutAsync();

        return TypedResults.Ok(MessageApi.UserLogOut);
    }

    private async Task<string> GenerateCallBackUrlAsync(string userId, string token)
    {
        var request = httpContextAccessor.HttpContext!.Request;
        var callbackUrl = $"{request.Scheme}://{request.Host}{EndpointsApi.EndpointsAccountGroup}" +
        $"{EndpointsApi.EndpointsConfirmEmail}".Replace("{userId}", userId).Replace("{token}", token);

        await Task.Delay(500);
        return callbackUrl;
    }

    private async Task<IdentityResult> AddClaimsToAdminUserAsync(ApplicationUser user)
    {
        //var claims = new List<Claim>
        //{
        //    //new(CustomClaimTypes.FullName, $"{user.FirstName} {user.LastName}")
        //};

        var claims = new List<Claim>();

        foreach (var permission in Enum.GetValues<Permissions>())
        {
            if (permission.ToString().Contains("Profile"))
            {
                var customClaim = new Claim(CustomClaimTypes.Permission, permission.ToString());
                claims.Add(customClaim);
            }

            //TODO: To be fixed when the permissions are implemented

            //var customClaim = new Claim(CustomClaimTypes.Permission, permission.ToString());
            //claims.Add(customClaim);
        }

        var result = await userManager.AddClaimsAsync(user, claims);

        return result;
    }
}