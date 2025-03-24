using MinimalApi.Identity.API.Entities.Common;

namespace MinimalApi.Identity.API.Entities;

public class ClaimType : BaseEntity
{
    public string Type { get; set; } = null!;
    public string Value { get; set; } = null!;
    public bool Default { get; set; }
}