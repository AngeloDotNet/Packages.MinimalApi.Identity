using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MinimalApi.Identity.API.Extensions;

public static class ServicesExtensions
{
    public static TOptions AddOptionValidate<TOptions>(this IServiceCollection services, string sectionName) where TOptions : class
    {
        services.AddOptions<TOptions>().BindConfiguration(sectionName).ValidateDataAnnotations().ValidateOnStart();

        using var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<TOptions>>().Value;

        return options;
    }

    public static string GetDatabaseConnString(this IConfiguration configuration, string sectionName)
    {
        var options = configuration.GetConnectionString(sectionName)
            ?? throw new ArgumentNullException(nameof(sectionName), "Connection string not found");

        return options;
    }

    public static IServiceCollection AddRegisterService<TAssembly>(this IServiceCollection services, string stringEndsWith,
        ServiceLifetime lifetime) where TAssembly : class
    {
        services.Scan(scan =>
            scan.FromAssemblyOf<TAssembly>()
                .AddClasses(classes => classes.Where(type => type.Name.EndsWith(stringEndsWith)))
                .AsImplementedInterfaces()
                .WithLifetime(lifetime));

        return services;
    }

    public static IServiceCollection AddRegisterTransientService<TAssembly>(this IServiceCollection services, string stringEndsWith)
        where TAssembly : class
    {
        return services.AddRegisterService<TAssembly>(stringEndsWith, ServiceLifetime.Transient);
    }

    public static IServiceCollection AddRegisterScopedService<TAssembly>(this IServiceCollection services, string stringEndsWith)
        where TAssembly : class
    {
        return services.AddRegisterService<TAssembly>(stringEndsWith, ServiceLifetime.Scoped);
    }

    public static IServiceCollection AddRegisterSingletonService<TAssembly>(this IServiceCollection services, string stringEndsWith)
        where TAssembly : class
    {
        return services.AddRegisterService<TAssembly>(stringEndsWith, ServiceLifetime.Singleton);
    }
}