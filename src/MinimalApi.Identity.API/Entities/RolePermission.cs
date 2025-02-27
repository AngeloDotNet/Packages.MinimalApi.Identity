namespace MinimalApi.Identity.API.Entities;

public class RolePermission
{
    public int RoleId { get; set; }
    public ApplicationRole Role { get; set; } = null!;

    public int PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;
}