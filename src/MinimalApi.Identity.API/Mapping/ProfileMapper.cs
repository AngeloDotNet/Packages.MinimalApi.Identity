using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Models;

namespace MinimalApi.Identity.API.Mapping;

internal static class ProfileMapper
{
    internal static UserProfileModel FromEntity(this UserProfile userProfile)
    {
        return new UserProfileModel(
            userProfile.UserId,
            userProfile.User.Email!,
            userProfile.FirstName,
            userProfile.LastName,
            userProfile.IsEnabled,
            userProfile.LastDateChangePassword
        );
    }
}
