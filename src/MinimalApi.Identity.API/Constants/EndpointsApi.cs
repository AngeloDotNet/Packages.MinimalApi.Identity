namespace MinimalApi.Identity.API.Constants;

public static class EndpointsApi
{
    public const string EndpointsDefaultApi = "/api";
    public const string EndpointsStringEmpty = "";

    public const string EndpointsAuthGroup = EndpointsDefaultApi + "/auth";
    public const string EndpointsAccountGroup = EndpointsDefaultApi + "/account";
    public const string EndpointsLicenzeGroup = EndpointsDefaultApi + "/licenses";
    public const string EndpointsModulesGroup = EndpointsDefaultApi + "/modules";
    //public const string EndpointsPermissionsGroup = EndpointsDefaultApi + "/permissions"; //TODO: Implement claims endpoints
    public const string EndpointsProfilesGroup = EndpointsDefaultApi + "/profiles";
    public const string EndpointsRolesGroup = EndpointsDefaultApi + "/roles";

    public const string EndpointsAuthTag = "Authentication";
    public const string EndpointsAccountTag = "Account";
    public const string EndpointsLicenzeTag = "Licenses";
    public const string EndpointsModulesTag = "Modules";
    //public const string EndpointsPermissionsTag = "Permissions"; //TODO: Implement claims endpoints tag
    public const string EndpointsProfilesTag = "Profiles";
    public const string EndpointsRolesTag = "Roles";

    public const string EndpointsAuthRegister = "/register";
    public const string EndpointsAuthLogin = "/login";
    public const string EndpointsAuthLogout = "/logout";
    public const string EndpointsForgotPassword = "/forgot-password";
    public const string EndpointsResetPassword = "/reset-password";
    public const string EndpointChangeEmail = "/change-email";
    public const string EndpointsConfirmEmail = "/confirm-email/{userId}/{token}";
    public const string EndpointsConfirmEmailChange = "/confirm-email-change/{userId}/{email}/{token}";

    public const string EndpointsGetProfile = "/{userId}";
    public const string EndpointsCreateProfile = "/create-profile";
    public const string EndpointsEditProfile = "/edit-profile";
    public const string EndpointsDeleteProfile = "/delete-profile";

    public const string EndpointsCreateLicense = "/create-license";
    public const string EndpointsAssignLicense = "/assign-license";
    public const string EndpointsRevokeLicense = "/revoke-license";
    public const string EndpointsDeleteLicense = "/delete-license";

    public const string EndpointsCreateModule = "/create-module";
    public const string EndpointsAssignModule = "/assign-module";
    public const string EndpointsRevokeModule = "/revoke-module";
    public const string EndpointsDeleteModule = "/delete-module";

    public const string EndpointsCreateRole = "/create-role";
    public const string EndpointsAssignRole = "/assign-role";
    public const string EndpointsRevokeRole = "/revoke-role";
    public const string EndpointsDeleteRole = "/delete-role";
}
