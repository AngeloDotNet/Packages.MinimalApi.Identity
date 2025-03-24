using MinimalApi.Identity.API.Entities.Common;

namespace MinimalApi.Identity.API.Entities;

public class AuthPolicy : BaseEntity
{
    public string PolicyName { get; set; } = null!;
    public string PolicyDescription { get; set; } = null!;
    public string[] PolicyPermissions { get; set; } = null!;
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
}