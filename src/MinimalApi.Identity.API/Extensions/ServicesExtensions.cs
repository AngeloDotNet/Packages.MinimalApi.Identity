using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MinimalApi.Identity.API.Extensions;

public static class ServicesExtensions
{
    public static T GetSettingsOptions<T>(this IConfiguration configuration, string sectionName) where T : class, new()
    {
        var options = configuration.GetSection(sectionName).Get<T>()
            ?? throw new ArgumentNullException(nameof(sectionName), $"{sectionName} not found");

        return options;
    }

    public static string GetDatabaseConnString(this IConfiguration configuration, string sectionName)
    {
        var options = configuration.GetConnectionString(sectionName)
            ?? throw new ArgumentNullException(nameof(sectionName), "Connection string not found");

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

    public static IServiceCollection AddRegisterScopedService<TAssembly>(this IServiceCollection services, string stringEndsWith) where TAssembly : class
    {
        services.Scan(scan =>
            scan.FromAssemblyOf<TAssembly>()
                .AddClasses(classes => classes.Where(type => type.Name.EndsWith(stringEndsWith)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());
        return services;
    }

    public static IServiceCollection AddRegisterSingletonService<TAssembly>(this IServiceCollection services, string stringEndsWith) where TAssembly : class
    {
        services.Scan(scan =>
            scan.FromAssemblyOf<TAssembly>()
                .AddClasses(classes => classes.Where(type => type.Name.EndsWith(stringEndsWith)))
                .AsImplementedInterfaces()
                .WithSingletonLifetime());
        return services;
    }
}