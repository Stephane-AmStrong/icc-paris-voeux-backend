using Application.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Services;

public class IccBootstrapService(IServiceProvider serviceProvider, ILogger<IccBootstrapService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("IccBootstrapService is starting.");

        using var scope = serviceProvider.CreateScope();

        await BootstrapServerSyncAsync(scope, stoppingToken);

        logger.LogInformation("IccBootstrapService completed successfully.");
    }

    private async Task BootstrapServerSyncAsync(IServiceScope scope, CancellationToken stoppingToken)
    {
        try
        {
            var serverEnvironmentSyncService = scope.ServiceProvider.GetRequiredService<IServerEnvironmentSyncService>();
            long result = await serverEnvironmentSyncService.SyncServersFromFlatConfigAsync(stoppingToken);

            logger.LogInformation("Initial server sync completed with {Count} updates", result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to perform initial server sync");
        }
    }
}
