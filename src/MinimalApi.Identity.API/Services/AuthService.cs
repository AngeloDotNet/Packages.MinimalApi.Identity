using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MinimalApi.Identity.API.Constants;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Enums;
using MinimalApi.Identity.API.Exceptions.BadRequest;
using MinimalApi.Identity.API.Exceptions.NotFound;
using MinimalApi.Identity.API.Exceptions.Users;
using MinimalApi.Identity.API.Extensions;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Options;
using MinimalApi.Identity.API.Services.Interfaces;

namespace MinimalApi.Identity.API.Services;

public class AuthService(IOptions<JwtOptions> jOptions, IOptions<NetIdentityOptions> iOptions, IOptions<UsersOptions> uOptions,
    UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSenderService emailSender,
    IHttpContextAccessor httpContextAccessor, ILicenseService licenseService, IModuleService moduleService, IProfileService profileService) : IAuthService
{
    public async Task<AuthResponseModel> LoginAsync(LoginModel model)
    {
        var identityOptions = iOptions.Value;
        var jwtOptions = jOptions.Value;
        var userOptions = uOptions.Value;

        var signInResult = await signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe,
            identityOptions.AllowedForNewUsers);

        if (!signInResult.Succeeded)
        {
            return signInResult switch
            {
                { IsNotAllowed: true } => throw new BadRequestUserException(MessageApi.UserNotAllowedLogin),
                { IsLockedOut: true } => throw new UserIsLockedException(MessageApi.UserLockedOut),
                { RequiresTwoFactor: true } => throw new BadRequestUserException(MessageApi.RequiredTwoFactor),
                _ => throw new BadRequestUserException(MessageApi.InvalidCredentials)
            };
        }

        var user = await userManager.FindByNameAsync(model.Username)
            ?? throw new NotFoundUserException(MessageApi.UserNotFound);

        if (!user.EmailConfirmed)
        {
            throw new BadRequestUserException(MessageApi.UserNotEmailConfirmed);
        }

        var profileUser = await profileService.GetProfileAsync(user.Id)
            ?? throw new NotFoundProfileException(MessageApi.ProfileNotFound);

        if (!profileUser.IsEnabled)
        {
            throw new BadRequestProfileException(MessageApi.UserNotEnableLogin);
        }

        var lastDateChangePassword = profileUser.LastDateChangePassword;
        var checkLastDateChangePassword = CheckLastDateChangePassword(lastDateChangePassword, userOptions);

        if (lastDateChangePassword == null || checkLastDateChangePassword)
        {
            throw new BadRequestProfileException(MessageApi.UserForcedChangePassword);
        }

        if (await licenseService.CheckUserLicenseExpiredAsync(user))
        {
            throw new BadRequestLicenseException(MessageApi.LicenseExpired);
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

        var loginResponse = CreateToken(claims, jwtOptions);

        user.RefreshToken = loginResponse.RefreshToken;
        user.RefreshTokenExpirationDate = DateTime.UtcNow.AddMinutes(60);

        await userManager.UpdateAsync(user);

        return loginResponse;
    }

    public async Task<string> RegisterAsync(RegisterModel model)
    {
        var usersOptions = uOptions.Value;
        var user = new ApplicationUser
        {
            UserName = model.Username,
            Email = model.Email
        };

        var result = await userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await profileService.CreateProfileAsync(new CreateUserProfileModel(user.Id, model.Firstname, model.Lastname));

            var role = await CheckUserIsAdminDesignedAsync(user.Email, usersOptions) ? DefaultRoles.Admin : DefaultRoles.User;
            var roleAssignResult = await userManager.AddToRoleAsync(user, role.ToString());

            if (!roleAssignResult.Succeeded)
            {
                throw new BadRequestRoleException(MessageApi.RoleNotAssigned);
            }

            var claimsAssignResult = await AddClaimsToUserAsync(user, role);

            if (!claimsAssignResult.Succeeded)
            {
                throw new BadRequestClaimException(MessageApi.ClaimsNotAssigned);
            }

            var userId = await userManager.GetUserIdAsync(user);
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var callbackUrl = await GenerateCallBackUrlAsync(userId, token);

            await emailSender.SendEmailTypeAsync(user.Email, callbackUrl, EmailSendingType.RegisterUser);

            return MessageApi.UserCreated;
        }

        throw new BadRequestUserException(result.Errors);
    }

    public async Task<AuthResponseModel> RefreshTokenAsync(RefreshTokenModel model)
    {
        var jwtOptions = jOptions.Value;
        var user = ValidateAccessToken(model.AccessToken)
            ?? throw new BadRequestUserException(MessageApi.InvalidAccessToken);

        var userId = user.GetUserId();
        var dbUser = await userManager.FindByIdAsync(userId);

        if (dbUser?.RefreshToken == null || dbUser.RefreshTokenExpirationDate <= DateTime.UtcNow || dbUser.RefreshToken != model.RefreshToken)
        {
            throw new BadRequestUserException(MessageApi.InvalidRefreshToken);
        }

        var loginResponse = CreateToken(user.Claims.ToList(), jwtOptions);

        dbUser.RefreshToken = loginResponse.RefreshToken;
        dbUser.RefreshTokenExpirationDate = DateTime.UtcNow.AddMinutes(jwtOptions.RefreshTokenExpirationMinutes);

        await userManager.UpdateAsync(dbUser);

        return loginResponse;
    }

    public async Task<string> LogoutAsync()
    {
        await signInManager.SignOutAsync();

        return MessageApi.UserLogOut;
    }

    public async Task<AuthResponseModel> ImpersonateAsync(ImpersonateUserModel inputModel)
    {
        var jwtOptions = jOptions.Value;
        var user = await userManager.FindByIdAsync(inputModel.UserId.ToString())
            ?? throw new UserUnknownException($"User not found");

        if (user.LockoutEnd.GetValueOrDefault() > DateTimeOffset.UtcNow)
        {
            throw new UserIsLockedException(MessageApi.UserLockedOut);
        }

        await userManager.UpdateSecurityStampAsync(user);

        var userRoles = await userManager.GetRolesAsync(user);
        var userClaims = await userManager.GetClaimsAsync(user);

        var customClaims = await GetCustomClaimsUserAsync(user);
        var identity = UsersExtensions.GetIdentity(httpContextAccessor);

        UpdateClaim(ClaimTypes.NameIdentifier, user.Id.ToString());
        UpdateClaim(ClaimTypes.Name, user.UserName ?? string.Empty);
        UpdateClaim(ClaimTypes.Email, user.Email ?? string.Empty);
        UpdateClaim(ClaimTypes.SerialNumber, user.SecurityStamp!.ToString());

        var updateIdentity = identity.Claims
            .Union(userClaims)
            .Union(customClaims)
            .Union(userRoles.Select(role => new Claim(ClaimTypes.Role, role))).ToList();

        var loginResponse = CreateToken(updateIdentity, jwtOptions);

        user.RefreshToken = loginResponse.RefreshToken;
        user.RefreshTokenExpirationDate = DateTime.UtcNow.AddMinutes(jwtOptions.RefreshTokenExpirationMinutes);

        await userManager.UpdateAsync(user);

        return loginResponse;

        void UpdateClaim(string type, string value)
        {
            var existingClaim = identity.FindFirst(type);

            if (existingClaim != null)
            {
                identity.RemoveClaim(existingClaim);
            }

            identity.AddClaim(new Claim(type, value));
        }
    }

    private static AuthResponseModel CreateToken(List<Claim> claims, JwtOptions jwtOptions)
    {
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

    private async Task<bool> CheckUserIsAdminDesignedAsync(string email, UsersOptions userOptions)
    {
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
        var claims = Enum.GetValues<Permissions>()
            .Where(claim => claim.ToString().Contains("profilo", StringComparison.InvariantCultureIgnoreCase))
            .Select(claim => new Claim(CustomClaimTypes.Permission, claim.ToString()))
            .ToList();

        return await userManager.AddClaimsAsync(user, claims);
    }

    private async Task<List<Claim>> GetCustomClaimsUserAsync(ApplicationUser user)
    {
        var customClaims = new List<Claim>();
        var userProfile = new List<Claim>();
        Claim? userClaimLicense = null;
        var userClaimModules = new List<Claim>();

        var task = new List<Task> {
            Task.Run(async () => userProfile = await profileService.GetClaimUserProfileAsync(user)),
            Task.Run(async () => userClaimLicense = await licenseService.GetClaimLicenseUserAsync(user)),
            Task.Run(async () => userClaimModules = await moduleService.GetClaimsModuleUserAsync(user))
        };

        await Task.WhenAll(task);

        if (userProfile != null)
        {
            customClaims.AddRange(userProfile);
        }

        if (userClaimLicense != null)
        {
            customClaims.Add(userClaimLicense);
        }

        if (userClaimModules?.Count > 0)
        {
            customClaims.AddRange(userClaimModules);
        }

        return customClaims;
    }

    private ClaimsPrincipal ValidateAccessToken(string accessToken)
    {
        var jwtOptions = jOptions.Value;
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecurityKey)),
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.Zero
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var user = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var securityToken);

            if (securityToken is JwtSecurityToken jwtSecurityToken && jwtSecurityToken.Header.Alg == SecurityAlgorithms.HmacSha256)
            {
                return user;
            }
        }
        catch
        {
            // Token validation failed
            // Log the exception if needed
        }

        // Token is invalid or expired
        // Log the invalid token if needed
        return null!;
    }

    private static bool CheckLastDateChangePassword(DateOnly? lastDate, UsersOptions userOptions)
    {
        if (lastDate!.Value.AddDays(userOptions.PasswordExpirationDays) <= DateOnly.FromDateTime(DateTime.UtcNow))
        {
            return true;
        }

        return false;
    }
}