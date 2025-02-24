using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
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

        builder.Services.AddRegisterServices<Program>(connectionString, jwtOptions);
        //builder.Services.AddAuthorization();
        //builder.Services.AddScoped<IAuthorizationHandler, UserActiveHandler>();

        //builder.Services.AddAuthorization(options =>
        //{
        //    var policyBuilder = new AuthorizationPolicyBuilder().RequireAuthenticatedUser();
        //    policyBuilder.Requirements.Add(new UserActiveRequirement());
        //    options.FallbackPolicy = options.DefaultPolicy = policyBuilder.Build();

        //    //options.AddPolicy("AdministratorOrPowerUser", policy =>
        //    //{
        //    //    policy.RequireRole(RoleNames.Administrator, RoleNames.PowerUser);
        //    //});
        //});

        //builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
        //builder.Services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();

        builder.Services.AddEndpointsApiExplorer()
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

        builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true)
            .Configure<KestrelServerOptions>(builder.Configuration.GetSection("Kestrel"));

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors("cors");

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMapEndpoints();
        app.Run();
    }
}