using IdentityManager.API.Middleware;
using MinimalApi.Identity.API.Enums;
using MinimalApi.Identity.API.Extensions;

namespace IdentityManager.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var AuthConnection = builder.Configuration.GetDatabaseConnString("DefaultConnection");
        var formatErrorResponse = ErrorResponseFormat.List; // or ErrorResponseFormat.Default

        builder.Services.AddCors(options => options.AddPolicy("cors", builder
            => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

        //...

        //If there is a need to register additional services(transient, scoped, singleton) in dependency injection,
        //it is possible to use the related extension methods exposed by the library.

        //NOTE: Service has already been used within the library to register the necessary services,
        //it is recommended to use a different nomenclature.

        //Using an extension method of the Scrutor package, all found services ending with Service
        //will be recorded in the Transient lifecycle.
        //builder.Services.AddRegisterTransientService<IAuthService>("Service");

        //Using an extension method of the Scrutor package, all found services ending with Service
        //will be recorded in the Scoped lifecycle.
        //builder.Services.AddRegisterScopedService<IAuthService>("Service");

        //Using an extension method of the Scrutor package, all found services ending with Service
        //will be recorded in the Singleton lifecycle.
        //builder.Services.AddRegisterSingletonService<IAuthService>("Service");

        builder.Services.AddRegisterServices<Program>(builder.Configuration, AuthConnection, formatErrorResponse);
        builder.Services.AddAuthorization();
        //builder.Services.AddAuthorization(options =>
        //{
        //    // Adds default authorization policies
        //    options.AddDefaultAuthorizationPolicy();

        //    // Here you can add additional authorization policies
        //});

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
    }
}