namespace MinimalApi.Identity.API.Options;

public class UsersOptions
{
    public string AssignAdminRoleOnRegistration { get; init; } = null!;
    public int PasswordExpirationDays { get; init; }
}
