using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MinimalApi.Identity.BusinessLayer.Handlers;
using MinimalApi.Identity.BusinessLayer.Options;
using MinimalApi.Identity.BusinessLayer.Services;
using MinimalApi.Identity.BusinessLayer.Services.Interfaces;
using MinimalApi.Identity.Common.Extensions.Interfaces;
using MinimalApi.Identity.DataAccessLayer;
using MinimalApi.Identity.DataAccessLayer.Entities;

namespace MinimalApi.Identity.Common.Extensions;

public static class RegisterServicesExtensions
{
    public static IServiceCollection AddRegisterServices<TMigrations>(this IServiceCollection services, string connectionString,
        JwtOptions jwtOptions) where TMigrations : class
    {
        services.AddMinimalApiDbContext(connectionString, typeof(TMigrations).Assembly.FullName!);
        services.AddMinimalApiIdentityServices(jwtOptions);
        services.AddMinimalApiIdentityOptionsServices();

        services.AddScoped<IAuthorizationHandler, PermissionHandler>();

        services.AddTransient<IEmailSender, EmailSender>();

        services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();

        return services;
    }

    public static void UseMapEndpoints(this WebApplication app) => app.MapEndpoints();

    internal static IServiceCollection AddMinimalApiDbContext(this IServiceCollection services, string connectionString, string migrationAssembly)
    {
        services.AddDbContext<MinimalApiDbContext>(options =>
        {
            options.UseSqlServer(connectionString, opt =>
            {
                opt.MigrationsAssembly(migrationAssembly);
                opt.MigrationsHistoryTable("MigrationsHistory");
                opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });
        });

        return services;
    }

    internal static IServiceCollection AddMinimalApiIdentityServices(this IServiceCollection services, JwtOptions jwtOptions)
    {
        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<MinimalApiDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer("Bearer", options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecurityKey)),
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        return services;
    }

    internal static IServiceCollection AddMinimalApiIdentityOptionsServices(this IServiceCollection services)
    {
        return services.Configure<IdentityOptions>(options =>
        {
            options.User.RequireUniqueEmail = true;

            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredUniqueChars = 4;

            options.SignIn.RequireConfirmedEmail = true;

            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        }).AddAuthorization();
    }
}