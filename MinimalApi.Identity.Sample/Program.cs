using MinimalApi.Identity.API.Extensions;
using MinimalApi.Identity.API.Options;

namespace MinimalApi.Identity.Sample;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration.GetDatabaseConnString("DefaultConnection");

        builder.Services.AddCors(options => options.AddPolicy("cors", builder
            => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

        var jwtOptions = builder.Configuration.GetSettingsOptions<JwtOptions>(nameof(JwtOptions));
        var identityOptions = builder.Configuration.GetSettingsOptions<NetIdentityOptions>(nameof(NetIdentityOptions));
        var smtpOptions = builder.Configuration.GetSettingsOptions<SmtpOptions>(nameof(SmtpOptions));

        builder.Services.AddRegisterServices<Program>(connectionString, jwtOptions, identityOptions)
            .AddAuthorization(options =>
            {
                options.AddDefaultAuthorizationPolicy(); // Adds default authorization policies
                // Here you can add additional authorization policies
            });

        builder.Services
            .AddSwaggerConfiguration()
            .AddRegisterOptions(builder.Configuration);

        //builder.Services.AddProblemDetails(options =>
        //{
        //	options.CustomizeProblemDetails = context =>
        //	{
        //		var problemDetails = context.ProblemDetails;
        //		var httpContext = context.HttpContext;
        //		var response = httpContext.Response;

        //		problemDetails.Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}";

        //		var activity = httpContext.Features.Get<IHttpActivityFeature>()?.Activity;

        //		switch (response.StatusCode)
        //		{
        //			case StatusCodes.Status401Unauthorized:
        //				problemDetails.Extensions.TryAdd("details", MessageApi.UserNotAuthenticated);
        //				problemDetails.Status = StatusCodes.Status401Unauthorized;
        //				break;
        //			case StatusCodes.Status412PreconditionFailed:
        //				problemDetails.Extensions.TryAdd("details", MessageApi.UserNotHavePermission);
        //				problemDetails.Status = StatusCodes.Status412PreconditionFailed;
        //				break;
        //		}

        //		problemDetails.Extensions.TryAdd("traceId", activity?.Id);
        //		problemDetails.Extensions.TryAdd("requestId", httpContext.TraceIdentifier);
        //	};
        //});

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger().UseSwaggerUI();
        }

        //app.UseExceptionHandler();
        //app.UseStatusCodePages();

        app.UseCors("cors");

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMapEndpoints();
        app.Run();
    }
}