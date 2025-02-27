using Microsoft.AspNetCore.Identity;

namespace MinimalApi.Identity.API.Entities;

public class ApplicationRole : IdentityRole<int>
{
    public ApplicationRole()
    { }

    public ApplicationRole(string roleName) : base(roleName)
    { }

    public ICollection<ApplicationUserRole> UserRoles { get; set; }
    public ICollection<RolePermission> RolePermissions { get; set; } = [];
}