using Microsoft.EntityFrameworkCore;
using MinimalApi.Identity.BusinessLayer.Services.Interfaces;
using MinimalApi.Identity.DataAccessLayer;

namespace MinimalApi.Identity.BusinessLayer.Services;

public class AuthService(MinimalApiDbContext dbContext) : IAuthService
{
    public async Task<IList<string>> GetPermissionsFromRolesAsync(IList<string> roles)
    {
        var roleIds = await dbContext.Roles
            .Where(r => roles.Contains(r.Name!))
            .Select(r => r.Id)
            .ToListAsync();

        var rolePermissions = await dbContext.RolePermissions
            .Where(rp => roleIds.Contains(rp.RoleId))
            .Select(rp => rp.Permission.Name)
            .ToListAsync();

        return rolePermissions;
    }
}