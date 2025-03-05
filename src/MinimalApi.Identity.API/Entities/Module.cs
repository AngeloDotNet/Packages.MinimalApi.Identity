namespace MinimalApi.Identity.API.Entities;

public class Module
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;

    public ICollection<UserModule> UserModules { get; set; } = [];
}