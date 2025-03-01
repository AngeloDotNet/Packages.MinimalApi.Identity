namespace MinimalApi.Identity.API.Models;

public record class AssignLicenseModel(int UserId, int LicenseId);
public record class AssignModuleModel(int UserId, int ModuleId);
public record class AssignPermissionModel(int RoleId, int PermissionId);
public record class AssignRoleModel(string Username, string Role);