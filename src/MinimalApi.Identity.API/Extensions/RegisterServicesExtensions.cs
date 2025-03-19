using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using MinimalApi.Identity.API.Authorization.Handlers;
using MinimalApi.Identity.API.Database;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Enums;
using MinimalApi.Identity.API.Filters;
using MinimalApi.Identity.API.Options;
using MinimalApi.Identity.API.Services.Interfaces;
using MinimalApi.Identity.API.Validator;
using MinimalApi.Identity.BusinessLayer.Authorization.Requirement;
using MinimalApi.Identity.Common.Extensions.Interfaces;

namespace MinimalApi.Identity.API.Extensions;

public static class RegisterServicesExtensions
{
    public static IServiceCollection AddRegisterServices<TMigrations>(this IServiceCollection services, IConfiguration configuration,
        string connectionString, JwtOptions jwtOptions, NetIdentityOptions identityOptions) where TMigrations : class
    {
        services
            .AddProblemDetails()
            .AddSwaggerConfiguration();

        services.AddMinimalApiDbContext(connectionString, typeof(TMigrations).Assembly.FullName!);
        services.AddMinimalApiIdentityServices(jwtOptions);
        services.AddMinimalApiIdentityOptionsServices(identityOptions);

        services
            .AddRegisterTransientService<IAuthService>("Service")

            .AddScoped<SignInManager<ApplicationUser>>()
            .AddScoped<IAuthorizationHandler, PermissionHandler>();

        services
            .ConfigureFluentValidation<LoginValidator>()
            .AddRegisterOptions(configuration);

        return services;
    }

    public static void UseMapEndpoints(this WebApplication app) => app.MapEndpoints();

    public static IServiceCollection AddRegisterOptions(this IServiceCollection services, IConfiguration configuration)
    {
        return services.Configure<JsonOptions>(options =>
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

    internal static IServiceCollection AddMinimalApiDbContext(this IServiceCollection services,
        string connectionString, string migrationAssembly)
    {
        services.AddDbContext<MinimalApiDbContext>(options =>
        {
            options.UseSqlServer(connectionString, opt =>
            {
                opt.MigrationsAssembly(migrationAssembly);
                opt.MigrationsHistoryTable(HistoryRepository.DefaultTableName);
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
        var permissionReadRequirement = new PermissionRequirement(nameof(Permissions.Claim), nameof(Permissions.ClaimRead));
        var permissionWriteRequirement = new PermissionRequirement(nameof(Permissions.Claim), nameof(Permissions.ClaimWrite));

        options.AddPolicy(nameof(Permissions.ClaimRead), policy => policy.Requirements.Add(permissionReadRequirement));
        options.AddPolicy(nameof(Permissions.ClaimWrite), policy => policy.Requirements.Add(permissionWriteRequirement));

        var licenseReadRequirement = new PermissionRequirement(nameof(Permissions.Licenza), nameof(Permissions.LicenzaRead));
        var licenseWriteRequirement = new PermissionRequirement(nameof(Permissions.Licenza), nameof(Permissions.LicenzaWrite));

        options.AddPolicy(nameof(Permissions.LicenzaRead), policy => policy.Requirements.Add(licenseReadRequirement));
        options.AddPolicy(nameof(Permissions.LicenzaWrite), policy => policy.Requirements.Add(licenseWriteRequirement));

        var moduleReadRequirement = new PermissionRequirement(nameof(Permissions.Modulo), nameof(Permissions.ModuloRead));
        var moduleWriteRequirement = new PermissionRequirement(nameof(Permissions.Modulo), nameof(Permissions.ModuloWrite));

        options.AddPolicy(nameof(Permissions.ModuloRead), policy => policy.Requirements.Add(moduleReadRequirement));
        options.AddPolicy(nameof(Permissions.ModuloWrite), policy => policy.Requirements.Add(moduleWriteRequirement));

        var profileReadRequirement = new PermissionRequirement(nameof(Permissions.Profilo), nameof(Permissions.ProfiloRead));
        var profileWriteRequirement = new PermissionRequirement(nameof(Permissions.Profilo), nameof(Permissions.ProfiloWrite));

        options.AddPolicy(nameof(Permissions.ProfiloRead), policy => policy.Requirements.Add(profileReadRequirement));
        options.AddPolicy(nameof(Permissions.ProfiloWrite), policy => policy.Requirements.Add(profileWriteRequirement));

        var roleReadRequirement = new PermissionRequirement(nameof(Permissions.Ruolo), nameof(Permissions.RuoloRead));
        var roleWriteRequirement = new PermissionRequirement(nameof(Permissions.Ruolo), nameof(Permissions.RuoloWrite));

        options.AddPolicy(nameof(Permissions.RuoloRead), policy => policy.Requirements.Add(roleReadRequirement));
        options.AddPolicy(nameof(Permissions.RuoloWrite), policy => policy.Requirements.Add(roleWriteRequirement));

        return options;
    }

    public static IServiceCollection AddRegisterTransientService<TAssembly>(this IServiceCollection services, string stringEndsWith) where TAssembly : class
    {
        services.Scan(scan =>
            scan.FromAssemblyOf<TAssembly>()
                .AddClasses(classes => classes.Where(type => type.Name.EndsWith(stringEndsWith)))
                .AsImplementedInterfaces()
                .WithTransientLifetime());

        return services;
    }

    public static RouteHandlerBuilder WithValidation<T>(this RouteHandlerBuilder builder) where T : class
        => builder.AddEndpointFilter<ValidatorFilter<T>>().ProducesValidationProblem();

    public static IServiceCollection ConfigureFluentValidation<TValidator>(this IServiceCollection services) where TValidator : IValidator
        => services.AddValidatorsFromAssemblyContaining<TValidator>();
}