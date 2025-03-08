using Microsoft.AspNetCore.Http;
using MinimalApi.Identity.API.Models;

namespace MinimalApi.Identity.API.Services.Interfaces;

public interface IProfileService
{
    Task<IResult> GetProfileAsync(string username);
    Task<IResult> EditProfileAsync(string username, UserProfileEditModel model);
    Task<IResult> DeleteProfileAsync(string username);
}
