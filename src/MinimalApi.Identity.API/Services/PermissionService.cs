using MinimalApi.Identity.API.Database;
using MinimalApi.Identity.API.Entities;

namespace MinimalApi.Identity.API.Services;

public class PermissionService(MinimalApiDbContext dbContext)
{
    public async Task AssignPermissionAsync(int userId, int permissionId)
    {
        var userPermission = new UserPermission
        {
            UserId = userId,
            PermissionId = permissionId
        };

        await dbContext.UserPermissions.AddAsync(userPermission);
        await dbContext.SaveChangesAsync();
    }
}