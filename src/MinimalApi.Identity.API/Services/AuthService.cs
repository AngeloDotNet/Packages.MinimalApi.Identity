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
    SignInManager<ApplicationUser> signInManager, IEmailSenderService emailSender, IHttpContextAccessor httpContextAccessor,
    ILicenseService licenseService, IModuleService moduleService, IProfileService profileService) : IAuthService
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

        var customClaims = await GetCustomClaimsUserAsync(user);
        var claims = new List<Claim>()
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.SerialNumber, user.SecurityStamp!.ToString()),
        }
        .Union(userClaims)
        .Union(customClaims)
        .Union(userRoles.Select(role => new Claim(ClaimTypes.Role, role))).ToList();

        var loginResponse = CreateToken(claims, configuration);

        //user.RefreshToken = loginResponse.RefreshToken;
        //user.RefreshTokenExpirationDate = DateTime.UtcNow.AddMinutes(60);

        await userManager.UpdateAsync(user);

        return TypedResults.Ok(loginResponse);
    }

    public async Task<IResult> RegisterAsync(RegisterModel model)
    {
        var user = new ApplicationUser
        {
            UserName = model.Username,
            Email = model.Email
        };

        var result = await userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await profileService.CreateProfileAsync(new CreateUserProfileModel(user.Id, model.FirstName, model.LastName));

            var role = await CheckUserIsAdminDesignedAsync(user.Email) ? DefaultRoles.Admin : DefaultRoles.User;
            var roleAssignResult = await userManager.AddToRoleAsync(user, role.ToString());

            if (!roleAssignResult.Succeeded)
            {
                return TypedResults.BadRequest(MessageApi.RoleNotAssigned);
            }

            var claimsAssignResult = await AddClaimsToUserAsync(user, role);

            if (!claimsAssignResult.Succeeded)
            {
                return TypedResults.BadRequest(MessageApi.ClaimsNotAssigned);
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

    private static AuthResponseModel CreateToken(IList<Claim> claims, IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSettingsOptions<JwtOptions>(nameof(JwtOptions));

        var audienceClaim = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Aud);
        claims.Remove(audienceClaim!);

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecurityKey));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(jwtOptions.Issuer, jwtOptions.Audience, claims,
            DateTime.UtcNow, DateTime.UtcNow.AddMinutes(jwtOptions.AccessTokenExpirationMinutes), signingCredentials);

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

    private async Task<string> GenerateCallBackUrlAsync(string userId, string token)
    {
        var request = httpContextAccessor.HttpContext!.Request;
        var callbackUrl = $"{request.Scheme}://{request.Host}{EndpointsApi.EndpointsAccountGroup}" +
        $"{EndpointsApi.EndpointsConfirmEmail}".Replace("{userId}", userId).Replace("{token}", token);

        await Task.Delay(500);
        return callbackUrl;
    }

    private async Task<bool> CheckUserIsAdminDesignedAsync(string email)
    {
        var userOptions = configuration.GetSettingsOptions<UsersOptions>(nameof(UsersOptions));
        var user = await userManager.FindByEmailAsync(email);

        if (user?.Email == null)
        {
            return false;
        }

        return user.Email.Equals(userOptions.AssignAdminRoleOnRegistration, StringComparison.InvariantCultureIgnoreCase);
    }

    private async Task<IdentityResult> AddClaimsToUserAsync(ApplicationUser user, DefaultRoles role)
    {
        return role switch
        {
            DefaultRoles.Admin => await AddClaimsToAdminUserAsync(user),
            DefaultRoles.User => await AddClaimsToDefaultUserAsync(user),
            _ => IdentityResult.Failed()
        };
    }

    private async Task<IdentityResult> AddClaimsToAdminUserAsync(ApplicationUser user)
    {
        var claims = Enum.GetValues<Permissions>()
            .Select(claim => new Claim(CustomClaimTypes.Permission, claim.ToString()))
            .ToList();

        return await userManager.AddClaimsAsync(user, claims);
    }

    private async Task<IdentityResult> AddClaimsToDefaultUserAsync(ApplicationUser user)
    {
        //Assign only read permissions to default user
        //var claims = Enum.GetValues<Permissions>()
        //    .Where(claim => !claim.ToString().Contains("write", StringComparison.InvariantCultureIgnoreCase))
        //    .Select(claim => new Claim(CustomClaimTypes.Permission, claim.ToString()))
        //    .ToList();

        //Assign only profile permissions to default user
        var claims = Enum.GetValues<Permissions>()
            .Where(claim => claim.ToString().Contains("profilo", StringComparison.InvariantCultureIgnoreCase))
            .Select(claim => new Claim(CustomClaimTypes.Permission, claim.ToString()))
            .ToList();

        return await userManager.AddClaimsAsync(user, claims);
    }

    private async Task<IList<Claim>> GetCustomClaimsUserAsync(ApplicationUser user)
    {
        var customClaims = new List<Claim>();

        var userProfile = await profileService.GetClaimUserProfileAsync(user);

        if (userProfile != null)
        {
            customClaims.AddRange(userProfile);
        }

        var userClaimLicense = await licenseService.GetClaimLicenseUserAsync(user);

        if (userClaimLicense != null)
        {
            customClaims.Add(userClaimLicense);
        }

        var userClaimModules = await moduleService.GetClaimsModuleUserAsync(user);

        if (userClaimModules?.Count > 0)
        {
            customClaims.AddRange(userClaimModules);
        }

        return customClaims;
    }
}