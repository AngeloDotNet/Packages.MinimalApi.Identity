using Microsoft.AspNetCore.Identity;

namespace MinimalApi.Identity.API.Entities;

public class ApplicationRole : IdentityRole<int>
{
    public ApplicationRole()
    { }

    public ApplicationRole(string roleName) : base(roleName)
    { }

    public bool Default { get; set; } = false;
    public ICollection<ApplicationUserRole> UserRoles { get; set; }
}