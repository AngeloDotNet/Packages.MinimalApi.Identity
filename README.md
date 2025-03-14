# Packages MinimalApi Identity API

### 🏗️ ToDo

- [ ] Add endpoints to handle refresh token (currently generated, but not usable)
- [ ] Add endpoints for two-factor authentication
- [ ] Add endpoint for forgotten password recovery
- [ ] Add endpoint for password change
- [ ] Add endpoints for two-factor authentication management
- [ ] Add endpoints for downloading and deleting personal data
- [ ] Add endpoints to manage claims and role (possible only if the data is not default)
- [ ] Add endpoints to manage users (including one to impersonate the user)
- [ ] Add validation input models
- [ ] Add missing documentation
- [ ] Refactoring code for manage claims (register and login), licenses and modules

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
        "Sender": "MyApplication <noreply@example.org>",
        "SaveEmailSent": false 
    },
    "UsersOptions": {
        "AssignAdminRoleOnRegistration": "admin@example.org"
    },
    "ConnectionStrings": {
        "DefaultConnection": "Data Source=[HOSTNAME];Initial Catalog=[DATABASE];User ID=[USERNAME];Password=[PASSWORD];Encrypt=False"
    }
}
```

> **Note**: If SaveEmailSent is false, only emails that failed while sending will be saved, if SaveEmailSent is true, both emails that were sent successfully and emails that failed will be saved

Registering services at _Program.cs_ file:

```csharp
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetDatabaseConnString("DefaultConnection");

var jwtOptions = builder.Configuration.GetSettingsOptions<JwtOptions>(nameof(JwtOptions));
var identityOptions = builder.Configuration.GetSettingsOptions<NetIdentityOptions>(nameof(NetIdentityOptions));
var smtpOptions = builder.Configuration.GetSettingsOptions<SmtpOptions>(nameof(SmtpOptions));

builder.Services.AddCors(options => options.AddPolicy("cors", builder
    => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddProblemDetails(option =>
{
    // Adds default mappings for exceptions to problem details
    options.AddDefaultProblemDetailsOptions();

    // Here you can add additional exception mappings
});

builder.Services.AddRegisterServices<Program>(builder.Configuration, connectionString, jwtOptions, identityOptions);
builder.Services.AddAuthorization(options =>
{
    // Adds default authorization policies
    options.AddDefaultAuthorizationPolicy();

    // Here you can add additional authorization policies
});

var app = builder.Build();
app.UseRegisterAppServices();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger().UseSwaggerUI();
}

app.UseCors("cors");

app.UseAuthentication();
app.UseAuthorization();

app.UseMapEndpoints();
app.Run();
```

<!--
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
-->

### 📦 Dependencies

- [.NET8](https://dotnet.microsoft.com/it-it/download/dotnet/8.0)
- [ASP.NET Core Identity](https://www.nuget.org/packages/Microsoft.AspNetCore.Identity.EntityFrameworkCore)
- [Entity Framework Core](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore)
- [Entity Framework Core for SQL Server](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SqlServer)
- [JWT Bearer](https://www.nuget.org/packages/Microsoft.AspNetCore.Authentication.JwtBearer)
- [MailKit](https://www.nuget.org/packages/MailKit)
- [Scrutor](https://www.nuget.org/packages/Scrutor)
- [Hellang Problem Details](https://www.nuget.org/packages/Hellang.Middleware.ProblemDetails)

### 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

### 📚 Demo

You can find a sample project in the [example](https://github.com/AngeloDotNet/IdentityManager) project.