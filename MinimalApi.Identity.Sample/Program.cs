//using Hellang.Middleware.ProblemDetails;
using MinimalApi.Identity.API.Extensions;
using MinimalApi.Identity.API.Options;
using MinimalApi.Identity.Sample.Middleware;

namespace MinimalApi.Identity.Sample;

public class Program
{
    public static void Main(string[] args)
    {
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

        app.UseMiddleware<ExtendedExceptionMiddleware>(); //Or app.UseMiddleware<MinimalApiExceptionMiddleware>();
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
    }
}