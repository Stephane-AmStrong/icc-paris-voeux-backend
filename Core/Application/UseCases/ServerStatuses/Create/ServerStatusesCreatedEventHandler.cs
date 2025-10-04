using Domain.Abstractions.Events;
using Domain.Events;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.ServerStatuses.Create;

public class ServerStatusCreatedEventHandler(ILogger<ServerStatusCreatedEventHandler> logger) : IEventHandler<ServerStatusCreatedEvent>
{
    public Task Handle(ServerStatusCreatedEvent createdEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation("ServerStatus successfully created - ServerStatusId: {ServerStatusId}, ServerStatusServerId: {ServerStatusServerId}, ServerStatusRecordedAt: {RecordedAt}", createdEvent.ServerStatus.Id, createdEvent.ServerStatus.ServerId, createdEvent.ServerStatus.RecordedAt);

        logger.LogDebug("ServerStatus deletion event processed for {ServerStatusId} at {ProcessedAt}", createdEvent.ServerStatus.Id, DateTime.UtcNow);

        return Task.CompletedTask;
    }
}
