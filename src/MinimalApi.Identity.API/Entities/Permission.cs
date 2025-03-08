using MinimalApi.Identity.API.Entities.Common;

namespace MinimalApi.Identity.API.Entities;

public class Permission : BaseEntity
{
    public string Name { get; set; } = null!;
    public bool Default { get; set; } = false;
    public ICollection<UserPermission> UserPermissions { get; set; } = [];
}