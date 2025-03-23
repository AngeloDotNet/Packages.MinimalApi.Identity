# Packages MinimalApi Identity API

Library for dynamically managing users, roles, claims, modules and license, using .NET 8 Minimal API, Entity Framework Core and SQL Server.

I created this library in order to avoid duplication of repetitive code whenever I implement Asp.Net Core Identity as an authentication and authorization provider

> **This library is still under development of new implementations.**

<!--
### 🏗️ ToDo

- [ ] Add endpoints to manage AuthPolicy data - If INSERT and/or UPDATE fails, throw ConstraintViolationException
- [ ] Add endpoints to manage AuthPolicyRoles data - If INSERT and/or UPDATE fails, throw ConstraintViolationException
- [ ] Add policies to the AuthPolicyRoles table  (https://github.com/ItalyDotNet/MyCourse/blob/master/src/MyCourse/Models/Services/Application/Courses/EfCoreCourseService.cs#L317)
- [ ] Resolve ToDo on line 206 of RegisterServicesExtensions
- [ ] Force automatic policy reload using Coravel (https://github.com/copilot/c/cd6ad08b-f6c8-4881-b40e-31b42c37183e)
- [ ] Verify the IResults of each service so that you have the XML documentation of the endpoints updated
- [ ] Add endpoints to manage users and disablement
- [ ] Add endpoints to handle user password change every X days
- [ ] Add endpoints to handle refresh token (currently generated, but not usable)
- [ ] Add endpoints to impersonate the user
- [ ] Add endpoint for forgotten password recovery
- [ ] Add endpoint for password change
- [ ] Add endpoints for two-factor authentication and management
- [ ] Add endpoints for downloading and deleting personal data
- [ ] Cleaning up commented code
- [ ] Add API documentation
-->

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
        "SecurityKey": "[SECURITY-KEY-512-CHAR]"
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
        "AssignAdminRoleOnRegistration": "admin@example.org"
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
        "MaxLengthClaimValue": 20
    },
    "ConnectionStrings": {
        "DefaultConnection": "Data Source=[HOSTNAME];Initial Catalog=[DATABASE];User ID=[USERNAME];Password=[PASSWORD];Encrypt=False"
    },
    "AllowedHosts": "*"
}
```

> **Note**: If SaveEmailSent is false, only emails that failed while sending will be saved, if SaveEmailSent is true, both emails that were sent successfully and emails that failed will be saved

Registering services at _Program.cs_ file:

```csharp
var builder = WebApplication.CreateBuilder(args);
var AuthConnection = builder.Configuration.GetDatabaseConnString("DefaultConnection");
var formatErrorResponse = ErrorResponseFormat.List; // or ErrorResponseFormat.Default

builder.Services.AddCors(options => options.AddPolicy("cors", builder
    => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

//...

//If there is a need to register additional services(transient, scoped, singleton) in dependency injection,
//it is possible to use the related extension methods exposed by the library.

//ATTENTION: Service has already been used within the library to register the necessary services, it is
//recommended to use a different nomenclature.

//Using an extension method of the Scrutor package, all found services ending with Service will be registered in the
//Transient lifecycle: builder.Services.AddRegisterTransientService<IAuthService>("Service"); or you can register
//services with the Scoped lifecycle: builder.Services.AddRegisterScopedService<IAuthService>("Service"); or you can
//register services with the Singleton lifecycle: builder.Services.AddRegisterSingletonService<IAuthService>("Service");

builder.Services.AddRegisterServices<Program>(builder.Configuration, AuthConnection, formatErrorResponse);
builder.Services.AddAuthorization(options =>
{
    // Adds default authorization policies
    options.AddDefaultAuthorizationPolicy();

    // Here you can add additional authorization policies
});

//...

var app = builder.Build();

//If you need to add more exceptions you need to add the ExtendedExceptionMiddleware middleware.
//In the demo project, in the Middleware folder, you can find an implementation example.
app.UseMiddleware<ExtendedExceptionMiddleware>();

//Otherwise you can add this middleware MinimalApiExceptionMiddleware to your pipeline
//that handles exceptions from this library.
//app.UseMiddleware<MinimalApiExceptionMiddleware>();

app.UseRouting();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger().UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", builder.Environment.ApplicationName);
    });
}

app.UseCors("cors");

app.UseAuthentication();
app.UseAuthorization();

app.UseMapEndpoints();
app.Run();
```

### 📚 Demo

You can find a sample project in the [example](https://github.com/AngeloDotNet/IdentityManager) project.

### 📦 Dependencies

- [.NET8](https://dotnet.microsoft.com/it-it/download/dotnet/8.0)
- [ASP.NET Core Identity](https://www.nuget.org/packages/Microsoft.AspNetCore.Identity.EntityFrameworkCore)
- [Entity Framework Core](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore)
- [Entity Framework Core for SQL Server](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SqlServer)
- [JWT Bearer](https://www.nuget.org/packages/Microsoft.AspNetCore.Authentication.JwtBearer)
- [MailKit](https://www.nuget.org/packages/MailKit)
- [Scrutor](https://www.nuget.org/packages/Scrutor)

### 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

### 🤝 Contributing

Suggestions are always welcome! Feel free to open suggestion issues in the repository and we will implement them as possible.