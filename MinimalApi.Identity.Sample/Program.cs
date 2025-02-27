using MinimalApi.Identity.BusinessLayer.Options;
using MinimalApi.Identity.Common.Extensions;

namespace MinimalApi.Identity.Sample;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentNullException("Connection string not found");

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddCors(options => options.AddPolicy("cors", builder
            => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

        var jwtOptions = builder.Configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>()
            ?? throw new ArgumentNullException("JWT options not found");

        var identityOptions = builder.Configuration.GetSection(nameof(NetIdentityOptions)).Get<NetIdentityOptions>()
            ?? throw new ArgumentNullException("Identity options not found");

        builder.Services.AddRegisterServices<Program>(connectionString, jwtOptions, identityOptions)
            .AddAuthorization(options =>
            {
                options.AddDefaultAuthorizationPolicy(); // Aggiunge le authorization policy di default
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