using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using MinimalApi.Identity.API.Authorization.Handlers;
using MinimalApi.Identity.API.Database;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Enums;
using MinimalApi.Identity.API.Options;
using MinimalApi.Identity.API.Services.Interfaces;
using MinimalApi.Identity.BusinessLayer.Authorization.Provider;
using MinimalApi.Identity.BusinessLayer.Authorization.Requirement;
using MinimalApi.Identity.Common.Extensions.Interfaces;

namespace MinimalApi.Identity.API.Extensions;

public static class RegisterServicesExtensions
{
    public static IServiceCollection AddRegisterServices<TMigrations>(this IServiceCollection services, string connectionString,
        JwtOptions jwtOptions, NetIdentityOptions identityOptions) where TMigrations : class
    {
        services.AddMinimalApiDbContext(connectionString, typeof(TMigrations).Assembly.FullName!, "MigrationsHistory");
        services.AddMinimalApiIdentityServices(jwtOptions);
        services.AddMinimalApiIdentityOptionsServices(identityOptions);

        services
            .AddRegisterTransientService<IAuthService>("Service")

            .AddScoped<SignInManager<ApplicationUser>>()
            .AddScoped<IAuthorizationHandler, CombinedPermissionHandler>()

            .AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();

        return services;
    }

    public static void UseMapEndpoints(this WebApplication app) => app.MapEndpoints();

    public static IServiceCollection AddRegisterOptions(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .Configure<JsonOptions>(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                options.JsonSerializerOptions.WriteIndented = true;
            })
            .Configure<RouteOptions>(options => options.LowercaseUrls = true)
            .Configure<KestrelServerOptions>(configuration.GetSection("Kestrel"));
    }

    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        return services
            .AddHttpContextAccessor()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Insert the Bearer Token",
                    Name = HeaderNames.Authorization,
                    Type = SecuritySchemeType.ApiKey
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference= new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
    }

    internal static IServiceCollection AddMinimalApiDbContext(this IServiceCollection services, string connectionString,
        string migrationAssembly, string migrationTableName)
    {
        services.AddDbContext<MinimalApiDbContext>(options =>
        {
            options.UseSqlServer(connectionString, opt =>
            {
                opt.MigrationsAssembly(migrationAssembly);
                opt.MigrationsHistoryTable(migrationTableName);
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

    internal static IServiceCollection AddMinimalApiIdentityOptionsServices(this IServiceCollection services, NetIdentityOptions identityOptions)
    {
        return services.Configure<IdentityOptions>(options =>
        {
            options.User.RequireUniqueEmail = identityOptions.RequireUniqueEmail;

            options.Password.RequireDigit = identityOptions.RequireDigit;
            options.Password.RequiredLength = identityOptions.RequiredLength;
            options.Password.RequireUppercase = identityOptions.RequireUppercase;
            options.Password.RequireLowercase = identityOptions.RequireLowercase;
            options.Password.RequireNonAlphanumeric = identityOptions.RequireNonAlphanumeric;
            options.Password.RequiredUniqueChars = identityOptions.RequiredUniqueChars;

            options.SignIn.RequireConfirmedEmail = identityOptions.RequireConfirmedEmail;

            options.Lockout.MaxFailedAccessAttempts = identityOptions.MaxFailedAccessAttempts;
            options.Lockout.AllowedForNewUsers = identityOptions.AllowedForNewUsers;
            options.Lockout.DefaultLockoutTimeSpan = identityOptions.DefaultLockoutTimeSpan;
        });
    }

    public static AuthorizationOptions AddDefaultAuthorizationPolicy(this AuthorizationOptions options)
    {
        var policies = new (string PolicyName, string RequirementName)[]
        {
            (nameof(Authorize.GetLicenses), nameof(Policy.Licenses)),
            (nameof(Authorize.CreateLicense), nameof(Policy.Licenses)),
            (nameof(Authorize.AssignLicense), nameof(Policy.Licenses)),
            (nameof(Authorize.DeleteLicense), nameof(Policy.Licenses)),

            (nameof(Authorize.GetModules), nameof(Policy.Modules)),
            (nameof(Authorize.CreateModule), nameof(Policy.Modules)),
            (nameof(Authorize.AssignModule), nameof(Policy.Modules)),
            (nameof(Authorize.DeleteModule), nameof(Policy.Modules)),

            (nameof(Authorize.GetPermissions), nameof(Policy.Permissions)),
            (nameof(Authorize.CreatePermission), nameof(Policy.Permissions)),
            (nameof(Authorize.AssignPermission), nameof(Policy.Permissions)),
            (nameof(Authorize.DeletePermission), nameof(Policy.Permissions)),

            (nameof(Authorize.GetRoles), nameof(Policy.Roles)),
            (nameof(Authorize.CreateRole), nameof(Policy.Roles)),
            (nameof(Authorize.AssignRole), nameof(Policy.Roles)),
            (nameof(Authorize.DeleteRole), nameof(Policy.Roles)),

            (nameof(Authorize.GetProfile), nameof(Policy.Profiles)),
            (nameof(Authorize.EditProfile), nameof(Policy.Profiles)),
            (nameof(Authorize.DeleteProfile), nameof(Policy.Profiles))
        };

        foreach (var (policyName, requirementName) in policies)
        {
            options.AddPolicy(policyName, policy
                => policy.Requirements.Add(new MultiPolicyRequirement(requirementName, policyName)));
        }

        return options;
    }
}