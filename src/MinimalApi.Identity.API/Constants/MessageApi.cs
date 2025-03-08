namespace MinimalApi.Identity.API.Constants;

public static class MessageApi
{
    public const string InvalidCredentials = "Invalid credentials";
    public const string UserNotAllowedLogin = "User is not allowed to sign in";
    public const string UserLogOut = "User logged out";
    public const string UserLockedOut = "This account has been locked out, please try again later";
    public const string UserNotEmailConfirmed = "User is not email confirmed";
    public const string RequiredTwoFactor = "Two-factor authentication is required";
    public const string ConfirmingEmail = "Thank you for confirming your email";
    public const string ConfirmingEmailChanged = "Thank you for confirming your email change";
    public const string ErrorConfirmEmail = "Error confirming your email";
    public const string ErrorConfirmEmailChange = "Error changing email";
    public const string ErrorChangeUsername = "Error changing username";
    public const string UserIdTokenRequired = "UserId and Token are required";
    public const string UserIdEmailTokenRequired = "UserId, Email and Token are required";
    public const string EmailUnchanged = "Your email is unchanged";
    public const string UserNotFound = "User not found";
    public const string SendEmailForChangeEmail = "Confirmation link to change email sent. Please check your email";
    public const string SendEmailResetPassword = "Email sent successfully";
    public const string ResetPassword = "Password reset successfully";
    public const string UserCreated = "User created successfully";
    public const string ProfileUpdated = "Profile updated successfully";
    public const string ProfileNotFound = "Profile not found";
    public const string UserDeleted = "User deleted successfully";
    public const string LicenseCreated = "License created successfully";
    public const string LicenseNotFound = "License not found";
    public const string LicenseAssigned = "License assigned successfully";
    public const string LicenseCanceled = "License removed successfully";
    public const string LicensesNotFound = "Licenses not found";
    public const string ModuleCreated = "Module created successfully";
    public const string ModuleNotFound = "Module not found";
    public const string ModuleAssigned = "Module assigned successfully";
    public const string ModuleCanceled = "Module removed successfully";
    public const string ModulesNotFound = "Modules not found";
    public const string PermissionCreated = "Permission created successfully";
    public const string PermissionNotFound = "Permission not found";
    public const string PermissionAssigned = "Permission assigned successfully";
    public const string PermissionCanceled = "Permission removed successfully";
    public const string PermissionsNotFound = "Permissions not found";
    public const string RoleCreated = "Role created successfully";
    public const string RoleNotFound = "Role not found";
    public const string RoleAssigned = "Role assigned successfully";
    public const string RoleCanceled = "Role removed successfully";
    public const string RoleExists = "Role already exists";
    public const string RolesNotFound = "Roles not found";
    public const string RoleNotAssigned = "Role not assigned";
    public const string ClaimsNotAssigned = "Claims not assigned";

    //Message exceptions
    public const string UserNotAuthenticated = "User is not authenticated";
    public const string UserNotHavePermission = "User does not have permission";
}