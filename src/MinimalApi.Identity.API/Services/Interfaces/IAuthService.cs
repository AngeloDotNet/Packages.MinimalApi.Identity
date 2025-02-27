namespace MinimalApi.Identity.API.Services.Interfaces;

public interface IAuthService
{
    Task<IList<string>> GetPermissionsFromRolesAsync(IList<string> roles);
}
