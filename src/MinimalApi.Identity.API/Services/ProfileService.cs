using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Identity.API.Constants;
using MinimalApi.Identity.API.Database;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Exceptions;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Services.Interfaces;

namespace MinimalApi.Identity.API.Services;

public class ProfileService(MinimalApiAuthDbContext dbContext, UserManager<ApplicationUser> userManager) : IProfileService
{
    //public async Task<IResult> GetProfileAsync(int userId)
    public async Task<UserProfileModel> GetProfileAsync(int userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundProfileException(MessageApi.ProfileNotFound);

        var profile = await dbContext.UserProfiles.AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == user.Id);

        //return profile == null ? TypedResults.NotFound(MessageApi.ProfileNotFound)
        //    : TypedResults.Ok(new UserProfileModel(profile.UserId, user.Email!, profile.FirstName, profile.LastName));
        return profile == null ? throw new NotFoundProfileException(MessageApi.ProfileNotFound) :
            new UserProfileModel(profile.UserId, user.Email!, profile.FirstName, profile.LastName, profile.IsEnabled, profile.LastDateChangePassword);
    }

    public async Task<string> CreateProfileAsync(CreateUserProfileModel model)
    {
        var profile = new UserProfile(model.UserId, model.FirstName, model.LastName);

        dbContext.UserProfiles.Add(profile);
        var result = await dbContext.SaveChangesAsync();

        return result > 0 ? MessageApi.ProfileCreated : MessageApi.ProfileNotCreated;
    }

    public async Task<string> EditProfileAsync(EditUserProfileModel model)
    {
        var profile = await dbContext.UserProfiles.AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == model.UserId)
            ?? throw new NotFoundProfileException(MessageApi.ProfileNotFound);

        profile.ChangeFirstName(model.FirstName);
        profile.ChangeLastName(model.LastName);

        dbContext.UserProfiles.Update(profile);
        var result = await dbContext.SaveChangesAsync();

        return result > 0 ? MessageApi.ProfileUpdated : throw new BadRequestProfileException(MessageApi.ProfileNotUpdated);
    }

    //public async Task<IResult> DeleteProfileAsync(DeleteUserProfileModel model)
    //{
    //    var user = await userManager.FindByIdAsync(model.UserId.ToString())
    //        ?? throw new NotFoundProfileException(MessageApi.ProfileNotFound);

    //    var result = await userManager.DeleteAsync(user);

    //    return result.Succeeded ? TypedResults.Ok(MessageApi.ProfileDeleted) : TypedResults.BadRequest(result.Errors.Select(e => e.Description));
    //}

    public async Task<IList<Claim>> GetClaimUserProfileAsync(ApplicationUser user)
    {
        var result = await dbContext.UserProfiles
            .AsNoTracking()
            .Where(ul => ul.UserId == user.Id)
            .Select(ul => new { ul.FirstName, ul.LastName })
            .FirstOrDefaultAsync();

        if (result == null)
        {
            return null!;
        }

        var profileClaims = new List<Claim>
        {
            new Claim(ClaimTypes.GivenName, result.FirstName ?? string.Empty),
            new Claim(ClaimTypes.Surname, result.LastName ?? string.Empty),
            new Claim(CustomClaimTypes.FullName, $"{result.FirstName} {result.LastName}")
        };

        return profileClaims;
    }

    public async Task<string> ChangeEnablementStatusUserProfileAsync(ChangeEnableProfileModel model)
    {
        var profile = await dbContext.UserProfiles.AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == model.UserId) ?? throw new NotFoundProfileException(MessageApi.ProfileNotFound);

        profile.ChangeUserEnabled(model.IsEnabled);

        dbContext.UserProfiles.Update(profile);
        var result = await dbContext.SaveChangesAsync();

        return result > 0 ? (model.IsEnabled ? MessageApi.ProfileEnabled : MessageApi.ProfileDisabled)
            : throw new BadRequestProfileException(model.IsEnabled ? MessageApi.ProfileNotEnabled : MessageApi.ProfileNotDisabled);
    }
}