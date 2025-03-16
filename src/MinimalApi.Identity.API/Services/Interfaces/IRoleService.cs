using Microsoft.AspNetCore.Http;
using MinimalApi.Identity.API.Models;

namespace MinimalApi.Identity.API.Services.Interfaces;

public interface IRoleService
{
    Task<IResult> GetAllRolesAsync();
    Task<IResult> CreateRoleAsync(CreateRoleModel model);
    Task<IResult> AssignRoleAsync(AssignRoleModel model);
    Task<IResult> RevokeRoleAsync(RevokeRoleModel model);
    Task<IResult> DeleteRoleAsync(DeleteRoleModel model);
}
