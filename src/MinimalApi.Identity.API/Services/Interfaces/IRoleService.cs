using MinimalApi.Identity.API.Models;

namespace MinimalApi.Identity.API.Services.Interfaces;

public interface IRoleService
{
    //Task<IResult> GetAllRolesAsync();
    Task<List<RoleResponseModel>> GetAllRolesAsync();
    //Task<IResult> CreateRoleAsync(CreateRoleModel model);
    Task<string> CreateRoleAsync(CreateRoleModel model);
    //Task<IResult> AssignRoleAsync(AssignRoleModel model);
    Task<string> AssignRoleAsync(AssignRoleModel model);
    //Task<IResult> RevokeRoleAsync(RevokeRoleModel model);
    Task<string> RevokeRoleAsync(RevokeRoleModel model);
    //Task<IResult> DeleteRoleAsync(DeleteRoleModel model);
    Task<string> DeleteRoleAsync(DeleteRoleModel model);
}
