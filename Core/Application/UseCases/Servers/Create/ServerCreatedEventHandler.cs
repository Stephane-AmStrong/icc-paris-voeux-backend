using Domain.Abstractions.Events;
using Domain.Events;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Servers.Create;

public class ServerCreatedEventHandler(ILogger<ServerCreatedEventHandler> logger) : IEventHandler<ServerCreatedEvent>
{
    public Task Handle(ServerCreatedEvent createdEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation("Server successfully created - Id: {Id}, HostName: {HostName}, Port: {Port}", createdEvent.Server.Id, createdEvent.Server.HostName, createdEvent.Server.Port);

        logger.LogDebug("Server creation event processed for {HostName} at {ProcessedAt}", createdEvent.Server.Id, DateTime.UtcNow);

        return Task.CompletedTask;
    }
}
