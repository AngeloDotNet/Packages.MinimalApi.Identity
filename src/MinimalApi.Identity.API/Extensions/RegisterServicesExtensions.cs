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
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using MinimalApi.Identity.API.Authorization.Handlers;
using MinimalApi.Identity.API.Database;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Enums;
using MinimalApi.Identity.API.Filters;
using MinimalApi.Identity.API.HostedServices;
using MinimalApi.Identity.API.Options;
using MinimalApi.Identity.API.Services.Interfaces;
using MinimalApi.Identity.API.Validator;
using MinimalApi.Identity.Common.Extensions.Interfaces;

namespace MinimalApi.Identity.API.Extensions;

public static class RegisterServicesExtensions
{
    public static IServiceCollection AddRegisterServices<TMigrations>(this IServiceCollection services, IConfiguration configuration,
        string dbConnString, ErrorResponseFormat formatErrorResponse) where TMigrations : class
    {
        var jwtOptions = configuration.GetSettingsOptions<JwtOptions>(nameof(JwtOptions));
        var identityOptions = configuration.GetSettingsOptions<NetIdentityOptions>(nameof(NetIdentityOptions));

        services
            .AddProblemDetails()
            .AddHttpContextAccessor()

            .AddSwaggerConfiguration()
            .AddMinimalApiDbContext<MinimalApiAuthDbContext>(dbConnString, typeof(TMigrations).Assembly.FullName!)
            .AddMinimalApiIdentityServices<MinimalApiAuthDbContext>(jwtOptions)
            .AddMinimalApiIdentityOptionsServices(identityOptions)

            .AddSingleton<IHostedService, AuthorizationPolicyGeneration>()
            .AddRegisterTransientService<IAuthService>("Service")
            .AddScoped<SignInManager<ApplicationUser>>()
            .AddScoped<IAuthorizationHandler, PermissionHandler>()

            .AddHostedService<AuthorizationPolicyUpdater>()

            .ConfigureValidation(options => options.ErrorResponseFormat = formatErrorResponse)
            .ConfigureFluentValidation<LoginValidator>()
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

        return services;
    }

    public static void UseMapEndpoints(this WebApplication app) => app.MapEndpoints();

    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        return services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen(options =>
            {
                var openApiInfo = new OpenApiInfo
                {
                    Title = "Minimal API Identity",
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = "Angelo Pirola",
                        Email = "angelo@aepserver.it",
                        Url = new Uri("https://angelo.aepserver.it/")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "License MIT",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    },
                };
                options.SwaggerDoc("v1", openApiInfo);

                var securityScheme = new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Insert the Bearer Token",
                    Name = HeaderNames.Authorization,
                    Type = SecuritySchemeType.ApiKey,
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = JwtBearerDefaults.AuthenticationScheme
                    }
                };
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);

                var securityRequirement = new OpenApiSecurityRequirement
                {
                    { securityScheme, Array.Empty<string>() }
                };
                options.AddSecurityRequirement(securityRequirement);
            });
    }

    public static RouteHandlerBuilder WithValidation<T>(this RouteHandlerBuilder builder) where T : class
        => builder.AddEndpointFilter<ValidatorFilter<T>>().ProducesValidationProblem();

    public static IServiceCollection ConfigureFluentValidation<TValidator>(this IServiceCollection services) where TValidator : IValidator
        => services.AddValidatorsFromAssemblyContaining<TValidator>();

    internal static IServiceCollection AddMinimalApiDbContext<TDbContext>(this IServiceCollection services, string dbConnString, string migrationAssembly)
        where TDbContext : DbContext
    {
        services.AddDbContext<TDbContext>(options =>
            options.UseSqlServer(dbConnString, opt =>
            {
                opt.MigrationsAssembly(migrationAssembly);
                opt.MigrationsHistoryTable(HistoryRepository.DefaultTableName);
                opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            })
        );

        return services;
    }

    internal static IServiceCollection AddMinimalApiIdentityServices<TDbContext>(this IServiceCollection services, JwtOptions jwtOptions)
        where TDbContext : DbContext
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecurityKey));

        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<TDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
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
                    IssuerSigningKey = key,
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        return services;
    }

    internal static IServiceCollection AddMinimalApiIdentityOptionsServices(this IServiceCollection services, NetIdentityOptions identityOptions)
    {
        services.Configure<IdentityOptions>(options =>
        {
            options.User.RequireUniqueEmail = identityOptions.RequireUniqueEmail;

            options.Password = new PasswordOptions
            {
                RequireDigit = identityOptions.RequireDigit,
                RequiredLength = identityOptions.RequiredLength,
                RequireUppercase = identityOptions.RequireUppercase,
                RequireLowercase = identityOptions.RequireLowercase,
                RequireNonAlphanumeric = identityOptions.RequireNonAlphanumeric,
                RequiredUniqueChars = identityOptions.RequiredUniqueChars
            };

            options.SignIn.RequireConfirmedEmail = identityOptions.RequireConfirmedEmail;

            options.Lockout = new LockoutOptions
            {
                MaxFailedAccessAttempts = identityOptions.MaxFailedAccessAttempts,
                AllowedForNewUsers = identityOptions.AllowedForNewUsers,
                DefaultLockoutTimeSpan = identityOptions.DefaultLockoutTimeSpan
            };
        });

        return services;
    }

    internal static IServiceCollection ConfigureValidation(this IServiceCollection services, Action<ValidationOptions> configureOptions)
    {
        services.Configure(configureOptions);
        return services;
    }
}