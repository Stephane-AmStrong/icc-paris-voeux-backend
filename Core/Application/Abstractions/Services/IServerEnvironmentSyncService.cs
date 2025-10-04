namespace Application.Abstractions.Services;

public interface IServerEnvironmentSyncService
{
    Task<long> SyncServersFromFlatConfigAsync(CancellationToken cancellationToken = default);
}
