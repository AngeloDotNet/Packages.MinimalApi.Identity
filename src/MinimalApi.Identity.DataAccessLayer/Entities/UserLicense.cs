namespace MinimalApi.Identity.DataAccessLayer.Entities;

public class UserLicense
{
    public int UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public int LicenseId { get; set; }
    public License License { get; set; } = null!;
}