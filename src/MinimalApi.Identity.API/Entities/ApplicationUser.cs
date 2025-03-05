using Microsoft.AspNetCore.Identity;

namespace MinimalApi.Identity.API.Entities;

public class ApplicationUser : IdentityUser<int>
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    //public string? RefreshToken { get; set; }
    //public DateTime? RefreshTokenExpirationDate { get; set; }

    public ICollection<ApplicationUserRole> UserRoles { get; set; } = [];
    public ICollection<UserLicense> UserLicenses { get; set; } = [];
    public ICollection<UserModule> UserModules { get; set; } = [];
}