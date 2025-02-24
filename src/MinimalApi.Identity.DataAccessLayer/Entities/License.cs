namespace MinimalApi.Identity.DataAccessLayer.Entities;

public class License
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime ExpirationDate { get; set; }
    public ICollection<UserLicense> UserLicenses { get; set; } = [];
}