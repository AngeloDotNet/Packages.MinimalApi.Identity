namespace MinimalApi.Identity.API.Models;

public record class RegisterModel(string Firstname, string Lastname, string Username, string Email, string Password);
public record class LoginModel(string Username, string Password, bool RememberMe);
//public record class ForgotPasswordModel(string Email);
//public record class ResetPasswordModel(string Email, string Token, string Password);
public record class ChangeEmailModel(string Email, string NewEmail);

public record class CreateUserProfileModel(int UserId, string FirstName, string LastName);
public record class EditUserProfileModel(int UserId, string FirstName, string LastName);
public record class ChangeEnableProfileModel(int UserId, bool IsEnabled);
public record class UserProfileModel(int UserId, string Email, string FirstName, string LastName, bool IsEnabled, DateOnly? LastDateChangePassword)
{
    public string FullName => $"{FirstName} {LastName}";
}

public record class CreateLicenseModel(string Name, DateOnly ExpirationDate);
public record class AssignLicenseModel(int UserId, int LicenseId);
public record class RevokeLicenseModel(int UserId, int LicenseId);
public record class DeleteLicenseModel(int LicenseId);

public record class CreateModuleModel(string Name, string Description);
public record class AssignModuleModel(int UserId, int ModuleId);
public record class RevokeModuleModel(int UserId, int ModuleId);
public record class DeleteModuleModel(int ModuleId);

public record class CreateRoleModel(string Role);
public record class AssignRoleModel(string Username, string Role);
public record class RevokeRoleModel(string Username, string Role);
public record class DeleteRoleModel(string Role);

public record class CreateClaimModel(string Type, string Value);
public record class AssignClaimModel(int UserId, string Type, string Value);
public record class RevokeClaimModel(int UserId, string Type, string Value);
public record class DeleteClaimModel(string Type, string Value);

public record class CreatePolicyModel(string PolicyName, string PolicyDescription, string[] PolicyPermissions);
public record class DeletePolicyModel(int Id, string PolicyName);