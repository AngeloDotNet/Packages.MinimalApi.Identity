﻿using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Identity.API.Constants;
using MinimalApi.Identity.API.Database;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Exceptions.BadRequest;
using MinimalApi.Identity.API.Exceptions.NotFound;
using MinimalApi.Identity.API.Mapping;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Services.Interfaces;

namespace MinimalApi.Identity.API.Services;

public class ProfileService(MinimalApiAuthDbContext dbContext, UserManager<ApplicationUser> userManager) : IProfileService
{
    public async Task<List<UserProfileModel>> GetProfilesAsync()
    {
        var profiles = await dbContext.UserProfiles.AsNoTracking()
            .Select(profile => ProfileMapper.FromEntity(profile))
            .ToListAsync();

        if (profiles.Count == 0)
        {
            throw new NotFoundProfileException(MessageApi.ProfilesNotFound);
        }

        return profiles;
    }

    public async Task<UserProfileModel> GetProfileAsync(int userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundProfileException(MessageApi.ProfileNotFound);

        var profile = await dbContext.UserProfiles.AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == user.Id)
            ?? throw new NotFoundProfileException(MessageApi.ProfileNotFound);

        return ProfileMapper.FromEntity(profile);
    }

    public async Task<string> CreateProfileAsync(CreateUserProfileModel model)
    {
        var profile = new UserProfile(model.UserId, model.FirstName, model.LastName);

        profile.ChangeUserEnabled(true);
        profile.ChangeLastDateChangePassword(DateOnly.FromDateTime(DateTime.Now));

        dbContext.UserProfiles.Add(profile);
        var result = await dbContext.SaveChangesAsync();

        return result > 0 ? MessageApi.ProfileCreated : MessageApi.ProfileNotCreated;
    }

    public async Task<string> EditProfileAsync(EditUserProfileModel model)
    {
        var profile = await dbContext.UserProfiles.FirstOrDefaultAsync(x => x.UserId == model.UserId)
            ?? throw new NotFoundProfileException(MessageApi.ProfileNotFound);

        profile.ChangeFirstName(model.FirstName);
        profile.ChangeLastName(model.LastName);

        dbContext.UserProfiles.Update(profile);
        var result = await dbContext.SaveChangesAsync();

        return result > 0 ? MessageApi.ProfileUpdated : throw new BadRequestProfileException(MessageApi.ProfileNotUpdated);
    }

    public async Task<List<Claim>> GetClaimUserProfileAsync(ApplicationUser user)
    {
        var result = await dbContext.UserProfiles
            .AsNoTracking()
            .Where(ul => ul.UserId == user.Id)
            .Select(ul => new { ul.FirstName, ul.LastName })
            .FirstOrDefaultAsync();

        if (result == null)
        {
            return [];
        }

        return
            [
                new Claim(ClaimTypes.GivenName, result.FirstName ?? string.Empty),
                new Claim(ClaimTypes.Surname, result.LastName ?? string.Empty),
                new Claim(CustomClaimTypes.FullName, $"{result.FirstName} {result.LastName}")
            ];
    }

    public async Task<string> ChangeEnablementStatusUserProfileAsync(ChangeEnableProfileModel model)
    {
        var profile = await dbContext.UserProfiles.FirstOrDefaultAsync(x => x.UserId == model.UserId)
            ?? throw new NotFoundProfileException(MessageApi.ProfileNotFound);

        profile.ChangeUserEnabled(model.IsEnabled);

        dbContext.UserProfiles.Update(profile);
        var result = await dbContext.SaveChangesAsync();

        return result > 0 ? (model.IsEnabled ? MessageApi.ProfileEnabled : MessageApi.ProfileDisabled)
            : throw new BadRequestProfileException(model.IsEnabled ? MessageApi.ProfileNotEnabled : MessageApi.ProfileNotDisabled);
    }
}