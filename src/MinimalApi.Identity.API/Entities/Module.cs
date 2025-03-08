using MinimalApi.Identity.API.Entities.Common;

namespace MinimalApi.Identity.API.Entities;

public class Module : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public ICollection<UserModule> UserModules { get; set; } = [];
}