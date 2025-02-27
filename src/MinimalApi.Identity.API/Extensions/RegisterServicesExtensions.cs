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
using MinimalApi.Identity.BusinessLayer.Authorization.Provider;
using MinimalApi.Identity.BusinessLayer.Authorization.Requirement;
using MinimalApi.Identity.BusinessLayer.Enums;
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
        JwtOptions jwtOptions, NetIdentityOptions identityOptions) where TMigrations : class
    {
        services.AddMinimalApiDbContext(connectionString, typeof(TMigrations).Assembly.FullName!);
        services.AddMinimalApiIdentityServices(jwtOptions);
        services.AddMinimalApiIdentityOptionsServices(identityOptions);

        services
            .AddTransient<IEmailSender, EmailSender>()
            .AddTransient<IAuthService, AuthService>()
            .AddScoped<IAuthorizationHandler, PermissionHandler>()
            .AddScoped<IAuthorizationHandler, MultiPermissionHandler>()
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
        //#region "Licenses"

        //options.AddPolicy(nameof(AuthPolicy.GetLicenses), policy
        //    => policy.Requirements.Add(new MultiPolicyRequirement(nameof(Policy.Licenses), nameof(AuthPolicy.GetLicenses))));

        //options.AddPolicy(nameof(AuthPolicy.CreateLicense), policy
        //    => policy.Requirements.Add(new MultiPolicyRequirement(nameof(Policy.Licenses), nameof(AuthPolicy.CreateLicense))));

        //options.AddPolicy(nameof(AuthPolicy.AssignLicense), policy
        //    => policy.Requirements.Add(new MultiPolicyRequirement(nameof(Policy.Licenses), nameof(AuthPolicy.AssignLicense))));

        //options.AddPolicy(nameof(AuthPolicy.DeleteLicense), policy
        //    => policy.Requirements.Add(new MultiPolicyRequirement(nameof(Policy.Licenses), nameof(AuthPolicy.DeleteLicense))));

        //#endregion

        //#region "Modules"

        //options.AddPolicy(nameof(AuthPolicy.GetModules), policy
        //    => policy.Requirements.Add(new MultiPolicyRequirement(nameof(Policy.Modules), nameof(AuthPolicy.GetModules))));

        //options.AddPolicy(nameof(AuthPolicy.CreateModule), policy
        //    => policy.Requirements.Add(new MultiPolicyRequirement(nameof(Policy.Modules), nameof(AuthPolicy.CreateModule))));

        //options.AddPolicy(nameof(AuthPolicy.AssignModule), policy
        //    => policy.Requirements.Add(new MultiPolicyRequirement(nameof(Policy.Modules), nameof(AuthPolicy.AssignModule))));

        //options.AddPolicy(nameof(AuthPolicy.DeleteModule), policy
        //    => policy.Requirements.Add(new MultiPolicyRequirement(nameof(Policy.Modules), nameof(AuthPolicy.DeleteModule))));

        //#endregion

        //#region "Permissions"

        //options.AddPolicy(nameof(AuthPolicy.GetPermissions), policy
        //    => policy.Requirements.Add(new MultiPolicyRequirement(nameof(Policy.Permissions), nameof(AuthPolicy.GetPermissions))));

        //options.AddPolicy(nameof(AuthPolicy.CreatePermission), policy
        //    => policy.Requirements.Add(new MultiPolicyRequirement(nameof(Policy.Permissions), nameof(AuthPolicy.CreatePermission))));

        //options.AddPolicy(nameof(AuthPolicy.AssignPermission), policy
        //    => policy.Requirements.Add(new MultiPolicyRequirement(nameof(Policy.Permissions), nameof(AuthPolicy.AssignPermission))));

        //options.AddPolicy(nameof(AuthPolicy.DeletePermission), policy
        //    => policy.Requirements.Add(new MultiPolicyRequirement(nameof(Policy.Permissions), nameof(AuthPolicy.DeletePermission))));

        //#endregion

        //#region "Roles"

        //options.AddPolicy(nameof(AuthPolicy.GetRoles), policy
        //    => policy.Requirements.Add(new MultiPolicyRequirement(nameof(Policy.Roles), nameof(AuthPolicy.GetRoles))));

        //options.AddPolicy(nameof(AuthPolicy.CreateRole), policy
        //    => policy.Requirements.Add(new MultiPolicyRequirement(nameof(Policy.Roles), nameof(AuthPolicy.CreateRole))));

        //options.AddPolicy(nameof(AuthPolicy.AssignRole), policy
        //    => policy.Requirements.Add(new MultiPolicyRequirement(nameof(Policy.Roles), nameof(AuthPolicy.AssignRole))));

        //options.AddPolicy(nameof(AuthPolicy.DeleteRole), policy
        //    => policy.Requirements.Add(new MultiPolicyRequirement(nameof(Policy.Roles), nameof(AuthPolicy.DeleteRole))));

        //#endregion

        //#region "Users"

        //options.AddPolicy(nameof(AuthPolicy.GetProfile), policy
        //    => policy.Requirements.Add(new MultiPolicyRequirement(nameof(Policy.Users), nameof(AuthPolicy.GetProfile))));

        //options.AddPolicy(nameof(AuthPolicy.EditProfile), policy
        //    => policy.Requirements.Add(new MultiPolicyRequirement(nameof(Policy.Users), nameof(AuthPolicy.EditProfile))));

        //options.AddPolicy(nameof(AuthPolicy.DeleteProfile), policy
        //    => policy.Requirements.Add(new MultiPolicyRequirement(nameof(Policy.Users), nameof(AuthPolicy.DeleteProfile))));

        //#endregion

        //return options;

        var policies = new (string PolicyName, string RequirementName)[]
        {
            (nameof(AuthPolicy.GetLicenses), nameof(Policy.Licenses)),
            (nameof(AuthPolicy.CreateLicense), nameof(Policy.Licenses)),
            (nameof(AuthPolicy.AssignLicense), nameof(Policy.Licenses)),
            (nameof(AuthPolicy.DeleteLicense), nameof(Policy.Licenses)),

            (nameof(AuthPolicy.GetModules), nameof(Policy.Modules)),
            (nameof(AuthPolicy.CreateModule), nameof(Policy.Modules)),
            (nameof(AuthPolicy.AssignModule), nameof(Policy.Modules)),
            (nameof(AuthPolicy.DeleteModule), nameof(Policy.Modules)),

            (nameof(AuthPolicy.GetPermissions), nameof(Policy.Permissions)),
            (nameof(AuthPolicy.CreatePermission), nameof(Policy.Permissions)),
            (nameof(AuthPolicy.AssignPermission), nameof(Policy.Permissions)),
            (nameof(AuthPolicy.DeletePermission), nameof(Policy.Permissions)),

            (nameof(AuthPolicy.GetRoles), nameof(Policy.Roles)),
            (nameof(AuthPolicy.CreateRole), nameof(Policy.Roles)),
            (nameof(AuthPolicy.AssignRole), nameof(Policy.Roles)),
            (nameof(AuthPolicy.DeleteRole), nameof(Policy.Roles)),

            //(nameof(AuthPolicy.GetProfile), nameof(Policy.Users)),
            //(nameof(AuthPolicy.EditProfile), nameof(Policy.Users)),
            //(nameof(AuthPolicy.DeleteProfile), nameof(Policy.Users))
        };

        foreach (var (policyName, requirementName) in policies)
        {
            options.AddPolicy(policyName, policy
                => policy.Requirements.Add(new MultiPolicyRequirement(requirementName, policyName)));
        }

        return options;
    }
}