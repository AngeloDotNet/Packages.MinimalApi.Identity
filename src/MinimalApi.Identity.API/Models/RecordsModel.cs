namespace MinimalApi.Identity.API.Models;

public record class RegisterModel(string FirstName, string LastName, string Username, string Email, string Password);
public record class LoginModel(string Username, string Password, bool RememberMe);
//public record class ForgotPasswordModel(string Email);
//public record class ResetPasswordModel(string Email, string Token, string Password);
public record class ChangeEmailModel(string Email, string NewEmail);
public record class UserProfileModel(int UserId, string Email, string FirstName, string LastName)
{
    public string FullName => $"{FirstName} {LastName}";
}

public record class CreateUserProfileModel(int UserId, string FirstName, string LastName);
public record class EditUserProfileModel(int UserId, string FirstName, string LastName);
public record class DeleteUserProfileModel(int UserId);

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

//RESPONSE RECORD MODEL
public record class AuthResponseModel(string AccessToken, string RefreshToken, DateTime ExpiredToken);
public record class LicenseResponseModel(int Id, string Name, DateOnly ExpirationDate);
public record class ModuleResponseModel(int Id, string Name, string Description);
public record class PermissionResponseModel(int Id, string Name, bool Default);
public record class RoleResponseModel(int Id, string Name, bool Default);
public record class ClaimResponseModel(int Id, string Type, string Value, bool Default);