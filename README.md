# Packages MinimalApi Identity API

Library for dynamically managing users, roles, claims, modules and license, using .NET 8 Minimal API, Entity Framework Core and SQL Server.

> [!IMPORTANT]
> **This library is still under development of new implementations.**

### 🏗️ ToDo

- [ ] Add endpoints for two-factor authentication and management
- [ ] Add endpoints for downloading and deleting personal data
- [ ] Test endpoints to impersonate the user
- [ ] Move email sending logic to a hosted service
- [ ] Add API documentation

### 🔜 Future implementations

- [ ] Replace exceptions with implementation of operation results 
- [ ] Replacing the hosted service email sender using Coravel jobs
- [ ] Replacing the hosted service authorization policy updater using Coravel jobs
- [ ] Add support for relational databases other than MS SQLServer (e.g. MySQL and PostgreSQL)
- [ ] Add support for multi tenancy
- [ ] Add authentication support from third-party providers (e.g. GitHub, Azure)

### 🛠️ Installation

The library is available on NuGet, just search for _MinimalApi.Identity.API_ in the Package Manager GUI or run the following command in the .NET CLI:

```shell
dotnet add package MinimalApi.Identity.API
```

### 🚀 Configuration

Adding this sections in the _appsettings.json_ file:

```json
{
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
        }
    },
    "Kestrel": {
        "Limits": {
            "MaxRequestBodySize": 5242880
        }
    },
    "JwtOptions": {
        "Issuer": "[ISSUER]",
        "Audience": "[AUDIENCE]",
        "SecurityKey": "[SECURITY-KEY-512-CHAR]",
        "AccessTokenExpirationMinutes": 60,
        "RefreshTokenExpirationMinutes": 60
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
        "SaveEmailSent": true
    },
    "UsersOptions": {
        "AssignAdminRoleOnRegistration": "admin@example.org",
        "PasswordExpirationDays": 90
    },
    "ApiValidationOptions": {
        "MinLengthFirstName": 3,
        "MaxLengthFirstName": 50,
        "MinLengthLastName": 3,
        "MaxLengthLastName": 50,
        "MinLengthUsername": 5,
        "MaxLengthUsername": 20,
        "MinLengthRoleName": 5,
        "MaxLengthRoleName": 20,
        "MinLengthModuleName": 5,
        "MaxLengthModuleName": 20,
        "MinLengthModuleDescription": 5,
        "MaxLengthModuleDescription": 100,
        "MinLengthLicenseName": 5,
        "MaxLengthLicenseName": 20,
        "MinLengthClaimValue": 5,
        "MaxLengthClaimValue": 20,
        "MinLengthPolicyName": 5,
        "MaxLengthPolicyName": 20,
        "MinLengthPolicyDescription": 5,
        "MaxLengthPolicyDescription": 100
    },
    "HostedServiceOptions": {
        "IntervalAuthPolicyUpdaterMinutes": 5
    },
    "ConnectionStrings": {
        "DefaultConnection": "Data Source=[HOSTNAME];Initial Catalog=[DATABASE];User ID=[USERNAME];Password=[PASSWORD];Encrypt=False"
    }
}
```

> [!IMPORTANT]
> If SaveEmailSent is false, only emails that failed while sending will be saved, if SaveEmailSent is true, both emails that were sent successfully and emails that failed will be saved

**Registering services at _Program.cs_ file**

```csharp
var builder = WebApplication.CreateBuilder(args);
var authConnection = builder.Configuration.GetDatabaseConnString("DefaultConnection");
var formatErrorResponse = ErrorResponseFormat.List; // or ErrorResponseFormat.Default

builder.Services.AddCors(options => options.AddPolicy("cors", builder
    => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

//...

//If you need to register additional services(transient, scoped, singleton) in dependency injection,
//you can use the related extension methods exposed by the library.

//NOTE: Service has already been used within the library to register the necessary services, it is
//recommended to use a different nomenclature.

//The library exposes the following extension methods that leverage the Scrutor package:
//- Transient lifecycle => builder.Services.AddRegisterTransientService<IAuthService>("Service");
//- Scoped lifecycle => builder.Services.AddRegisterScopedService<IAuthService>("Service");
//- Singleton lifecycle => builder.Services.AddRegisterSingletonService<IAuthService>("Service");

builder.Services.AddRegisterServices<Program>(builder.Configuration, authConnection, formatErrorResponse);
builder.Services.AddAuthorization(options =>
{
    // Here you can add additional authorization policies
});

//...

var app = builder.Build();
app.UseHttpsRedirection();

//Use this MinimalApiExceptionMiddleware in your pipeline if you don't need to add new exceptions.
app.UseMiddleware<MinimalApiExceptionMiddleware>();

//If you need to add more exceptions, you need to add the ExtendedExceptionMiddleware middleware to your pipeline.
//In the demo project, in the Middleware folder, you can find an example implementation, which you can use to add
//the exceptions you need.
//app.UseMiddleware<ExtendedExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger()
        .UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", builder.Environment.ApplicationName);
        });
}

app.UseStatusCodePages();
app.UseRouting();

app.UseCors("cors");

app.UseAuthentication();
app.UseAuthorization();

app.UseMapEndpoints();
app.Run();
```

### 📡 API Reference

See the [documentation](https://github.com/AngeloDotNet/Packages.MinimalApi.Identity/tree/main/docs) for a list of all available API endpoints.

### 📚 Demo

You can find a sample project in the [example](https://github.com/AngeloDotNet/Packages.MinimalApi.Identity/tree/main/IdentityManager.API) project.

### 📦 Dependencies

- [.NET 8](https://dotnet.microsoft.com/it-it/download/dotnet/8.0)
- [ASP.NET Core Identity](https://www.nuget.org/packages/Microsoft.AspNetCore.Identity.EntityFrameworkCore)
- [Entity Framework Core](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore)
- [Entity Framework Core for SQL Server](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SqlServer)
- [JWT Bearer Token](https://www.nuget.org/packages/Microsoft.AspNetCore.Authentication.JwtBearer)
- [MailKit](https://www.nuget.org/packages/MailKit)
- [Scrutor](https://www.nuget.org/packages/Scrutor)

### 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

### ⭐ Give a Star

Don't forget that if you find this project useful, put a ⭐ on GitHub to show your support and help others discover it.

### 🤝 Contributing

The project is constantly evolving. Contributions are always welcome. Feel free to report issues and pull requests on the repository.