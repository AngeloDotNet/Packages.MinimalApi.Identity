using MinimalApi.Identity.API.Models;

namespace MinimalApi.Identity.API.Services.Interfaces;

public interface IRoleService
{
    Task<List<RoleResponseModel>> GetAllRolesAsync();
    Task<string> CreateRoleAsync(CreateRoleModel model);
    Task<string> AssignRoleAsync(AssignRoleModel model);
    Task<string> RevokeRoleAsync(RevokeRoleModel model);
    Task<string> DeleteRoleAsync(DeleteRoleModel model);
}
