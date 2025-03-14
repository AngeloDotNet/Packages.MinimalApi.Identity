using Microsoft.AspNetCore.Identity;

namespace MinimalApi.Identity.API.Entities;

public class ApplicationUser : IdentityUser<int>
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    //public bool IsEnabled { get; set; }
    //public DateTime? LastChangePassword { get; set; }
    //public string RefreshToken { get; set; } = null!;
    //public DateTime? RefreshTokenExpirationDate { get; set; }
    public ICollection<ApplicationUserRole> UserRoles { get; set; } = [];
    //public ICollection<IdentityUserClaim<int>> UserClaims { get; set; } = [];
    public ICollection<ApplicationUserClaim> UserClaims { get; set; } = [];
    public ICollection<UserLicense> UserLicenses { get; set; } = [];
    public ICollection<UserModule> UserModules { get; set; } = [];
}