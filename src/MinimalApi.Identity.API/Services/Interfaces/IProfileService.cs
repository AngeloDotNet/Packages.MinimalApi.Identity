using System.Security.Claims;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Models;

namespace MinimalApi.Identity.API.Services.Interfaces;

public interface IProfileService
{
    //Task<IResult> GetProfileAsync(int userId);
    //Task<Results<Ok<UserProfileModel>, NotFound<string>>> GetProfileAsync(int userId);
    Task<List<UserProfileModel>> GetProfilesAsync();
    Task<UserProfileModel> GetProfileAsync(int userId);
    //Task<IResult> CreateProfileAsync(CreateUserProfileModel model);
    Task<string> CreateProfileAsync(CreateUserProfileModel model);
    //Task<IResult> EditProfileAsync(EditUserProfileModel model);
    Task<string> EditProfileAsync(EditUserProfileModel model);
    //Task<IResult> DeleteProfileAsync(DeleteUserProfileModel model);
    Task<IList<Claim>> GetClaimUserProfileAsync(ApplicationUser user);
    Task<string> ChangeEnablementStatusUserProfileAsync(ChangeEnableProfileModel model);
}
