using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Identity.API.Constants;
using MinimalApi.Identity.API.Database;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Services.Interfaces;

namespace MinimalApi.Identity.API.Services;

public class ProfileService(MinimalApiDbContext dbContext, UserManager<ApplicationUser> userManager) : IProfileService
{
    public async Task<IResult> GetProfileAsync(int userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            return TypedResults.NotFound(MessageApi.ProfileNotFound);
        }

        var profile = await dbContext.UserProfiles.AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == user.Id);

        return profile == null ? TypedResults.NotFound(MessageApi.ProfileNotFound)
            : TypedResults.Ok(new UserProfileModel(profile.UserId, user.Email!, profile.FirstName, profile.LastName));
    }

    public async Task<IResult> CreateProfileAsync(CreateUserProfileModel model)
    {
        var profile = new UserProfile
        {
            UserId = model.UserId,
            FirstName = model.FirstName,
            LastName = model.LastName
        };

        dbContext.UserProfiles.Add(profile);
        var result = await dbContext.SaveChangesAsync();

        return result > 0 ? TypedResults.Ok(MessageApi.ProfileCreated) : TypedResults.BadRequest(MessageApi.ProfileNotCreated);
    }

    public async Task<IResult> EditProfileAsync(EditUserProfileModel model)
    {
        var profile = await dbContext.UserProfiles.AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == model.UserId);

        if (profile == null)
        {
            return TypedResults.NotFound(MessageApi.ProfileNotFound);
        }

        profile.FirstName = model.FirstName;
        profile.LastName = model.LastName;

        //dbContext.Entry(profile).State = EntityState.Modified;
        dbContext.UserProfiles.Update(profile);
        var result = await dbContext.SaveChangesAsync();

        return result > 0 ? TypedResults.Ok(MessageApi.ProfileUpdated) : TypedResults.BadRequest(MessageApi.ProfileNotUpdated);

    }

    public async Task<IResult> DeleteProfileAsync(DeleteUserProfileModel model)
    {
        var user = await userManager.FindByIdAsync(model.UserId.ToString());

        if (user == null)
        {
            return TypedResults.NotFound(MessageApi.ProfileNotFound);
        }

        var result = await userManager.DeleteAsync(user);

        return result.Succeeded ? TypedResults.Ok(MessageApi.ProfileDeleted) : TypedResults.BadRequest(result.Errors.Select(e => e.Description));

    }

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
}
