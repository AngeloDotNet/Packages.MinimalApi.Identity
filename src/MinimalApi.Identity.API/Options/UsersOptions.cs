using System.ComponentModel.DataAnnotations;

namespace MinimalApi.Identity.API.Options;

public class UsersOptions
{
    [Required, MinLength(1, ErrorMessage = "AssignAdminRoleOnRegistration cannot be empty.")]
    public string AssignAdminRoleOnRegistration { get; init; } = null!;

    [Required, Range(1, int.MaxValue, ErrorMessage = "PasswordExpirationDays must be greater than zero.")]
    public int PasswordExpirationDays { get; init; }
}
