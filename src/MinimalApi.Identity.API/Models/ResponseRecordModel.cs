namespace MinimalApi.Identity.API.Models;

public record class AuthResponseModel(string AccessToken, string RefreshToken, DateTime ExpiredToken);
public record class LicenseResponseModel(int Id, string Name, DateOnly ExpirationDate);
public record class ModuleResponseModel(int Id, string Name, string Description);
public record class PermissionResponseModel(int Id, string Name, bool Default);
public record class RoleResponseModel(int Id, string Name, bool Default);
public record class ClaimResponseModel(int Id, string Type, string Value, bool Default);
public record class PolicyResponseModel(int Id, string PolicyName, string PolicyDescription, string[] PolicyPermissions);