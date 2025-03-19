using MinimalApi.Identity.API.Entities.Common;

namespace MinimalApi.Identity.API.Entities;

public class UserProfile : BaseEntity
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    //public bool IsEnabled { get; set; }
    //public DateTime? LastChangePassword { get; set; }
    public int UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
}