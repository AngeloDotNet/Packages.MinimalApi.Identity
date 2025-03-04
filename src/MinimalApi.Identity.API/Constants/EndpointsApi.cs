namespace MinimalApi.Identity.API.Constants;

public static class EndpointsApi
{
    public const string EndpointsStringEmpty = "";
    public const string EndpointsProfile = "/{username}";

    public const string EndpointsAuthGroup = "/authentication";
    public const string EndpointsAccountGroup = "/account";
    public const string EndpointsLicenzeGroup = "/licenses";
    public const string EndpointsModulesGroup = "/modules";
    public const string EndpointsPermissionsGroup = "/permissions";
    public const string EndpointsRolesGroup = "/roles";
    public const string EndpointsProfilesGroup = "/profiles";

    public const string EndpointsAuthTag = "Authentication";
    public const string EndpointsAccountTag = "Account";
    public const string EndpointsLicenzeTag = "Licenses";
    public const string EndpointsModulesTag = "Modules";
    public const string EndpointsPermissionsTag = "Permissions";
    public const string EndpointsRolesTag = "Roles";
    public const string EndpointsProfilesTag = "Profiles";

    public const string EndpointsAuthRegister = "/register";
    public const string EndpointsAuthLogin = "/login";
    public const string EndpointsForgotPassword = "/forgot-password";
    public const string EndpointsResetPassword = "/reset-password";

    public const string EndpointUserIdToken = "/{userId}/{token}";
    public const string EndpointsConfirmEmail = "/confirm-email" + EndpointUserIdToken;

    public const string EndpointsCreateLicense = "/create-license";
    public const string EndpointsAssignLicense = "/assign-license";
    public const string EndpointsRevokeLicense = "/revoke-license";

    public const string EndpointsCreateModule = "/create-module";
    public const string EndpointsAssignModule = "/assign-module";
    public const string EndpointsRevokeModule = "/revoke-module";

    public const string EndpointsCreatePermission = "/create-permission";
    public const string EndpointsAssignPermission = "/assign-permission";
    public const string EndpointsRevokePermission = "/revoke-permission";

    public const string EndpointsCreateRole = "/create-role";
    public const string EndpointsAssignRole = "/assign-role";
    public const string EndpointsRevokeRole = "/revoke-role";
}
