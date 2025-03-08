namespace MinimalApi.Identity.API.Enums;

public enum Authorize
{
    GetLicenses,
    CreateLicense,
    AssignLicense,
    DeleteLicense,

    GetModules,
    CreateModule,
    AssignModule,
    DeleteModule,

    GetPermissions,
    CreatePermission,
    AssignPermission,
    DeletePermission,

    GetRoles,
    CreateRole,
    AssignRole,
    DeleteRole,

    GetProfile,
    EditProfile,
    DeleteProfile
}