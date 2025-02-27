namespace MinimalApi.Identity.BusinessLayer.Services.Interfaces;

public interface IAuthService
{
    Task<IList<string>> GetPermissionsFromRolesAsync(IList<string> roles);
}
