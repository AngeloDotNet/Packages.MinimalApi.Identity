# Packages MinimalApi Identity API

### 🏗️ ToDo

- [ ] Add endpoints to handle refresh token (currently generated, but not usable)
- [ ] Add endpoints present in asp net core identity scaffolding
- [ ] Add endpoints to delete license, module, permission and role (possible only if the data is not default)
- [ ] Add endpoints to manage users (including one to impersonate the user)
- [ ] Add validation input models
- [ ] Add missing documentation

### 🛠️ Installation

The library is available on NuGet. Just search for MinimalApi.Identity.API in the Package Manager GUI or run the following command in the .NET CLI:

```shell
dotnet add package MinimalApi.Identity.API
```

### 🚀 Configuration

Adding this sections in the _appsettings.json_ file:

```json
{
    "Kestrel": {
        "Limits": {
            "MaxRequestBodySize": 5242880
        }
    },
    "JwtOptions": {
        "Issuer": "[ISSUER]",
        "Audience": "[AUDIENCE]",
        "SecurityKey": "[SECURITY-KEY]",
    },
    "NetIdentityOptions": {
        "RequireUniqueEmail": true,
        "RequireDigit": true,
        "RequiredLength": 8,
        "RequireUppercase": true,
        "RequireLowercase": true,
        "RequireNonAlphanumeric": true,
        "RequiredUniqueChars": 4,
        "RequireConfirmedEmail": true,
        "MaxFailedAccessAttempts": 3,
        "AllowedForNewUsers": true,
        "DefaultLockoutTimeSpan": "00:05:00"
    },
    "SmtpOptions": {
        "Host": "smtp.example.org",
        "Port": 25,
        "Security": "StartTls",
        "Username": "Username del server SMTP",
        "Password": "Password del server SMTP",
        "Sender": "MyApplication <noreply@example.org>"
    },
    "UsersOptions": {
        "AssignAdminRoleOnRegistration": "admin@example.org"
    },
    "ConnectionStrings": {
        "DefaultConnection": "Data Source=[HOSTNAME];Initial Catalog=[DATABASE];User ID=[USERNAME];Password=[PASSWORD];Encrypt=False"
    }
}
```

Registering services at startup:

```csharp
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetDatabaseConnString("DefaultConnection");

builder.Services.AddHttpContextAccessor();

//...

var jwtOptions = builder.Configuration.GetSettingsOptions<JwtOptions>(nameof(JwtOptions));
var identityOptions = builder.Configuration.GetSettingsOptions<NetIdentityOptions>(nameof(NetIdentityOptions));
var smtpOptions = builder.Configuration.GetSettingsOptions<SmtpOptions>(nameof(SmtpOptions));

builder.Services.AddRegisterServices<Program>(connectionString, jwtOptions, identityOptions)
    .AddAuthorization(options =>
    {
        options.AddDefaultAuthorizationPolicy(); // Adds default authorization policies

        // Here you can add additional authorization policies
    });

builder.Services.AddSwaggerConfiguration()
    .AddRegisterOptions(builder.Configuration);

var app = builder.Build();

//...

app.UseAuthentication();
app.UseAuthorization();

//...

app.UseMapEndpoints();
app.Run();
```

### 📡 API Reference

The library provides a series of endpoints to manage the identity of the application.

#### Confirm email address

```http
  GET /api/account/confirm-email/{userId}/{token}
```

| Parameter | Type     | Required |
| :-------- | :------- | :------- |
| `userId` | `string` | Yes |
| `token` | `string` | Yes |

#### Register a new user

```http
  POST /api/authentication/register
```

| Parameter | Type     | Required |
| :-------- | :------- | :------- |
| `firstName` | `string` | Yes |
| `lastName` | `string` | Yes |
| `username` | `string` | Yes |
| `email` | `string` | Yes |
| `password` | `string` | Yes |

#### Login user

```http
  POST /api/authentication/login
```

| Parameter | Type     | Required |
| :-------- | :------- | :------- |
| `username` | `string` | Yes |
| `password` | `string` | Yes |
| `rememberMe` | `bool` | Yes |

<!--
- **/api/licenses**: Get all licenses
- **/api/licenses/create-license**: Create a new license
- **/api/licenses/assign-license**: Assign a license to a user
- **/api/licenses/revoke-license**: Revoke a license from a user

- **/api/modules**: Get all modules
- **/api/modules/create-module**: Create a new module
- **/api/modules/assign-module**: Assign a module to a user
- **/api/modules/revoke-module**: Revoke a module from a user

- **/api/permissions**: Get all permissions
- **/api/permissions/create-permission**: Create a new permission
- **/api/permissions/assign-permission**: Assign a permission to a user
- **/api/permissions/revoke-permission**: Revoke a permission from a user
-->

#### Get user profile

```http
  GET /api/profiles/{username}
```

| Parameter | Type     | Required |
| :-------- | :------- | :------- |
| `username` | `string` | Yes |

#### Edit user profile

```http
  PUT /api/profiles/{username}
```

| Parameter | Type     | Required |
| :-------- | :------- | :------- |
| `username` | `string` | Yes |

#### Delete user profile

```http
  DELETE /api/profiles/{username}
```

| Parameter | Type     | Required |
| :-------- | :------- | :------- |
| `username` | `string` | Yes |

<!--
- **/api/roles**: Get all roles
- **/api/roles/create-role**: Create a new role
- **/api/roles/assign-role**: Assign a role to a user
- **/api/roles/revoke-role**: Revoke a role from a user
-->

### 📦 Dependencies

- [.NET8](https://dotnet.microsoft.com/it-it/download/dotnet/8.0)
- [ASP.NET Core Identity](https://www.nuget.org/packages/Microsoft.AspNetCore.Identity.EntityFrameworkCore)
- [Entity Framework Core](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore)
- [Entity Framework Core for SQL Server](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SqlServer)
- [JWT Bearer](https://www.nuget.org/packages/Microsoft.AspNetCore.Authentication.JwtBearer)
- [MailKit](https://www.nuget.org/packages/MailKit)

### 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

### 📚 Demo

You can find a sample project in the [example](https://github.com/AngeloDotNet/IdentityManager) project.