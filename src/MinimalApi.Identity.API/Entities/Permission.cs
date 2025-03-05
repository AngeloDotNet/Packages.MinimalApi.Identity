namespace MinimalApi.Identity.API.Entities;

public class Permission
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public bool Default { get; set; } = false;

    public ICollection<RolePermission> RolePermissions { get; set; } = [];
}