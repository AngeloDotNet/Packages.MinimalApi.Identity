using Microsoft.EntityFrameworkCore;
using MinimalApi.Identity.API.Database;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Services.Interfaces;

namespace MinimalApi.Identity.API.Services;

public class AuthService(MinimalApiDbContext dbContext) : IAuthService
{
    public async Task<IList<string>> GetPermissionsFromUserAsync(ApplicationUser user)
    {
        var permissions = await dbContext.UserPermissions
            .Where(up => up.UserId == user.Id)
            .Select(up => up.Permission.Name)
            .Distinct()
            .ToListAsync();

        return permissions;
    }
}