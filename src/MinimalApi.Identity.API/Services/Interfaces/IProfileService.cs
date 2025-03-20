using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Models;

namespace MinimalApi.Identity.API.Services.Interfaces;

public interface IProfileService
{
    Task<IResult> GetProfileAsync(int userId);
    Task<IResult> CreateProfileAsync(CreateUserProfileModel model);
    Task<IResult> EditProfileAsync(EditUserProfileModel model);
    Task<IResult> DeleteProfileAsync(DeleteUserProfileModel model);

    Task<IList<Claim>> GetClaimUserProfileAsync(ApplicationUser user);
}
