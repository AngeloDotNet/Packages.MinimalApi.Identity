using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using MinimalApi.Identity.API.Models;

namespace MinimalApi.Identity.API.Services.Interfaces;

public interface IProfileService
{
    Task<Results<Ok<UserProfileModel>, NotFound<string>>> GetProfileAsync(string username);
    Task<Results<Ok<string>, NotFound<string>, BadRequest<IEnumerable<IdentityError>>>> EditProfileAsync(string username, UserProfileEditModel model);
    Task<Results<Ok<string>, NotFound<string>, BadRequest<IEnumerable<IdentityError>>>> DeleteProfileAsync(string username);
}
