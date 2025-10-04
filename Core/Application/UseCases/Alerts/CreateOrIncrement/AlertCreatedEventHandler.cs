using Domain.Abstractions.Events;
using Domain.Events;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Alerts.CreateOrIncrement;

public class AlertCreatedEventHandler(ILogger<AlertCreatedEventHandler> logger) : IEventHandler<AlertCreatedEvent>
{
    public Task Handle(AlertCreatedEvent createdEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation("Alert successfully created - Id: {AlertId}, ServerId: {ServerId}, OccurredAt: {OccurredAt}", createdEvent.Alert.Id, createdEvent.Alert.ServerId, createdEvent.Alert.OccurredAt);

        logger.LogDebug("Alert creation event processed for {AlertId} at {ProcessedAt}", createdEvent.Alert.Id, DateTime.UtcNow);

        return Task.CompletedTask;
    }
}
