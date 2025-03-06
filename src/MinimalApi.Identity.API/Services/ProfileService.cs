using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using MinimalApi.Identity.API.Constants;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Services.Interfaces;

namespace MinimalApi.Identity.API.Services;

public class ProfileService(UserManager<ApplicationUser> userManager) : IProfileService
{
    public async Task<Results<Ok<UserProfileModel>, NotFound<string>>> GetProfileAsync(string username)
    {
        var user = await userManager.FindByNameAsync(username);

        return user == null ? TypedResults.NotFound(MessageApi.ProfileNotFound)
            : TypedResults.Ok(new UserProfileModel(username, user.Email!, user.FirstName, user.LastName));
    }

    public async Task<Results<Ok<string>, NotFound<string>, BadRequest<IEnumerable<IdentityError>>>> EditProfileAsync(string username, UserProfileEditModel model)
    {
        var user = await userManager.FindByNameAsync(username);

        if (user == null)
        {
            return TypedResults.NotFound(MessageApi.ProfileNotFound);
        }

        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.Email = model.Email;

        var result = await userManager.UpdateAsync(user);

        return result.Succeeded ? TypedResults.Ok(MessageApi.ProfileUpdated) : TypedResults.BadRequest(result.Errors);
    }

    public async Task<Results<Ok<string>, NotFound<string>, BadRequest<IEnumerable<IdentityError>>>> DeleteProfileAsync(string username)
    {
        var user = await userManager.FindByNameAsync(username);

        if (user == null)
        {
            return TypedResults.NotFound(MessageApi.ProfileNotFound);
        }

        var result = await userManager.DeleteAsync(user);

        return result.Succeeded ? TypedResults.Ok(MessageApi.UserDeleted) : TypedResults.BadRequest(result.Errors);
    }
}
