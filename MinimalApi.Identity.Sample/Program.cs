using MinimalApi.Identity.API.Extensions;
using MinimalApi.Identity.API.Options;

namespace MinimalApi.Identity.Sample;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration.GetDatabaseConnString("DefaultConnection");

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddCors(options => options.AddPolicy("cors", builder
            => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

        var jwtOptions = builder.Configuration.GetSettingsOptions<JwtOptions>(nameof(JwtOptions));
        var identityOptions = builder.Configuration.GetSettingsOptions<NetIdentityOptions>(nameof(NetIdentityOptions));
        var smtpOptions = builder.Configuration.GetSettingsOptions<SmtpOptions>(nameof(SmtpOptions));

        builder.Services.AddRegisterServices<Program>(connectionString, jwtOptions, identityOptions)
            .AddAuthorization(options =>
            {
                options.AddDefaultAuthorizationPolicy(); // Aggiunge le authorization policy di default

                // Qui è possibile aggiungere ulteriori authorization policy
            });

        builder.Services
            .AddSwaggerConfiguration()
            .AddRegisterOptions(builder.Configuration);

        var app = builder.Build();

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