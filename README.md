# Packages MinimalApi Identity API

Library for dynamically managing users, roles, claims, modules and license, using .NET 8 Minimal API, Entity Framework Core and SQL Server.

I created this library in order to avoid duplication of repetitive code whenever I implement Asp.Net Core Identity as an authentication and authorization provider

> [!IMPORTANT]
> **The MinimalApi.Identity.API library used in this sample project, is still under development of new implementations.**

### 🏗️ ToDo

- [ ] Add endpoints to manage claims (with possible deletion only if the data is not default)
- [ ] Add endpoints to manage users
- [ ] Add endpoints to impersonate the user
- [ ] Add endpoints to manage user disablement
- [ ] Add endpoints to handle user password change every X days
- [ ] Add endpoints to handle refresh token (currently generated, but not usable)
- [ ] Add endpoints for two-factor authentication
- [ ] Add endpoint for forgotten password recovery
- [ ] Add endpoint for password change
- [ ] Add endpoints for two-factor authentication management
- [ ] Add endpoints for downloading and deleting personal data
- [ ] Add API documentation
 
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

builder.Services.AddRegisterServices<Program>(builder.Configuration, connectionString, jwtOptions, identityOptions);
builder.Services.AddAuthorization(options =>
{
    // Adds default authorization policies
    options.AddDefaultAuthorizationPolicy();

    // Here you can add additional authorization policies
});

var app = builder.Build();

//If you need to add more exceptions you need to add the ExtendedExceptionMiddleware middleware.
//In the demo project, in the Middleware folder, you can find an implementation example.
app.UseMiddleware<ExtendedExceptionMiddleware>();

//Otherwise you can add this middleware MinimalApiExceptionMiddleware to your pipeline that handles exceptions from this library.
//app.UseMiddleware<MinimalApiExceptionMiddleware>();

app.UseRouting();

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


### 📚 Demo

You can find a sample project in the [example](https://github.com/AngeloDotNet/Packages.MinimalApi.Identity/tree/main/MinimalApi.Identity.Sample) project.

### 📦 Dependencies

- [.NET8](https://dotnet.microsoft.com/it-it/download/dotnet/8.0)
- [ASP.NET Core Identity](https://www.nuget.org/packages/Microsoft.AspNetCore.Identity.EntityFrameworkCore)
- [Entity Framework Core](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore)
- [Entity Framework Core for SQL Server](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SqlServer)
- [JWT Bearer](https://www.nuget.org/packages/Microsoft.AspNetCore.Authentication.JwtBearer)
- [MailKit](https://www.nuget.org/packages/MailKit)
- [Scrutor](https://www.nuget.org/packages/Scrutor)
<!--
- [Hellang Problem Details](https://www.nuget.org/packages/Hellang.Middleware.ProblemDetails)
-->

### 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

### ⭐ Give a Star

If you find this project useful, please give it a ⭐ on GitHub to show your support and help others discover it!

### 🤝 Contributing

Suggestions are always welcome! Feel free to open suggestion issues in the repository and we will implement them as possible.