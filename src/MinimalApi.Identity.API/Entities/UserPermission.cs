namespace MinimalApi.Identity.API.Entities;

public class UserPermission
{
    public int UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public int PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;
}
