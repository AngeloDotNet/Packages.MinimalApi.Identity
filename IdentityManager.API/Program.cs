using MinimalApi.Identity.API.Enums;
using MinimalApi.Identity.API.Extensions;
using MinimalApi.Identity.API.Middleware;

namespace IdentityManager.API;

public class Program
{
    public static void Main(string[] args)
    {
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
        app.UseMiddleware<MinimalApiExceptionMiddleware>(); //Use this middleware in your pipeline if you don't need to add new exceptions.

        //If you need to add more exceptions, you need to add the ExtendedExceptionMiddleware middleware to your pipeline.
        //In the demo project, in the Middleware folder, you can find an example implementation, which you can use to add
        //the exceptions you need.
        //app.UseMiddleware<ExtendedExceptionMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            var appName = builder.Environment.ApplicationName;

            app.UseSwagger()
                .UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", appName));
        }

        app.UseStatusCodePages();
        app.UseRouting();

        app.UseCors("cors");

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMapEndpoints();
        app.Run();
    }
}