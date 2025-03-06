using Microsoft.EntityFrameworkCore;
using MinimalApi.Identity.API.Services.Interfaces;

namespace MinimalApi.Identity.API.Services;

public class AuthService(MinimalApiDbContext dbContext) : IAuthService
{
    public async Task<IList<string>> GetPermissionsFromRolesAsync(IList<string> roles)
    {
        var roleIds = await dbContext.Roles
            .Where(r => roles.Contains(r.Name!))
            .Select(r => r.Id)
            .ToListAsync();

        var permissions = await dbContext.RolePermissions
            .Where(rp => roleIds.Contains(rp.RoleId))
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .ToListAsync();

        return permissions;
    }

    //public async Task<IList<string>> GetPermissionsFromUserAsync(ApplicationUser user)
    //{
    //    var permissions = await dbContext.UserPermissions
    //        .Where(up => up.UserId == user.Id)
    //        .Select(up => up.Permission.Name)
    //        .Distinct()
    //        .ToListAsync();

    //    return permissions;
    //}
}