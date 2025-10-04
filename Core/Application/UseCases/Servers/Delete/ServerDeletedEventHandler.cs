using Domain.Abstractions.Events;
using Domain.Events;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Servers.Delete;

public class ServerDeletedEventHandler(ILogger<ServerDeletedEventHandler> logger) : IEventHandler<ServerDeletedEvent>
{
    public Task Handle(ServerDeletedEvent deletedEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation("Server successfully deleted - Id: {HostName}, HostName: {HostName}, AppName: {AppName}", deletedEvent.Server.Id, deletedEvent.Server.HostName, deletedEvent.Server.AppName);

        logger.LogDebug("Server deletion event processed for {HostName} at {ProcessedAt}", deletedEvent.Server.Id, DateTime.UtcNow);

        return Task.CompletedTask;
    }
}
