using MinimalApi.Identity.DataAccessLayer.Entities;

namespace MinimalApi.Identity.Shared;

public class Permission
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<RolePermission> RolePermissions { get; set; } = [];
}