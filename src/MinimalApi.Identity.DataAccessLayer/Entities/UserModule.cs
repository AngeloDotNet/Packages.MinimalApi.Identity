namespace MinimalApi.Identity.DataAccessLayer.Entities;

public class UserModule
{
    public int UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public int ModuleId { get; set; }
    public Module Module { get; set; } = null!;
}