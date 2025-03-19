using Microsoft.Extensions.Configuration;

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
}