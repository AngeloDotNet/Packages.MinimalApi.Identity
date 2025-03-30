using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MinimalApi.Identity.API.Options;
using MinimalApi.Identity.API.Services.Interfaces;

namespace MinimalApi.Identity.API.HostedServices;

//public class AuthorizationPolicyUpdater(IServiceProvider serviceProvider, ILogger<AuthorizationPolicyUpdater> logger, IConfiguration configuration) : IHostedService, IDisposable
public class AuthorizationPolicyUpdater(IServiceProvider serviceProvider, ILogger<AuthorizationPolicyUpdater> logger,
    IOptions<HostedServiceOptions> hostedOptions) : IHostedService, IDisposable
{
    private Timer? timer;
    //private readonly HostedServiceOptions options = configuration.GetSettingsOptions<HostedServiceOptions>(nameof(HostedServiceOptions));
    private readonly HostedServiceOptions options = hostedOptions.Value;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        timer = new Timer(UpdateAuthorizationPolicyAsync, null, TimeSpan.Zero, TimeSpan.FromMinutes(options.IntervalAuthPolicyUpdaterMinutes));
        return Task.CompletedTask;
    }

    private async void UpdateAuthorizationPolicyAsync(object? state)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var authPolicyService = scope.ServiceProvider.GetRequiredService<IAuthPolicyService>();
            var result = await authPolicyService.UpdateAuthPoliciesAsync();

            if (result)
            {
                logger.LogInformation("Authorization policies updated successfully.");
            }
            else
            {
                logger.LogWarning("An error occurred while generating authorization policies.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred while updating authorization policies.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        timer?.Dispose();
    }
}