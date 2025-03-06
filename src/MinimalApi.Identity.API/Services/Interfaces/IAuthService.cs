using MinimalApi.Identity.API.Entities;

namespace MinimalApi.Identity.API.Services.Interfaces;

public interface IAuthService
{
    Task<IList<string>> GetPermissionsFromUserAsync(ApplicationUser user);
}