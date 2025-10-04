using Domain.Abstractions.Events;
using Domain.Events;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Servers.Update;

public class ServerUpdatedEventHandler(ILogger<ServerUpdatedEventHandler> logger) : IEventHandler<ServerUpdatedEvent>
{
    public Task Handle(ServerUpdatedEvent updatedEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation("Server successfully updated - Id: {HostName}, HostName: {HostName}, AppName: {AppName}", updatedEvent.Server.Id, updatedEvent.Server.HostName, updatedEvent.Server.AppName);

        logger.LogDebug("Server deletion event processed for {HostName} at {ProcessedAt}", updatedEvent.Server.Id, DateTime.UtcNow);

        return Task.CompletedTask;
    }
}
