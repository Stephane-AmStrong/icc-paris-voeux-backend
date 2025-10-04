using Domain.Abstractions.Events;
using Domain.Events;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Alerts.Update;

public class AlertUpdatedEventHandler(ILogger<AlertUpdatedEventHandler> logger) : IEventHandler<AlertUpdatedEvent>
{
    public Task Handle(AlertUpdatedEvent updatedEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation("Alert successfully updated - Id: {AlertId}, ServerId: {ServerId}, OccurredAt: {OccurredAt}", updatedEvent.Alert.Id, updatedEvent.Alert.ServerId, updatedEvent.Alert.OccurredAt);

        logger.LogDebug("Alert creation event processed for {AlertId} at {ProcessedAt}", updatedEvent.Alert.Id, DateTime.UtcNow);

        return Task.CompletedTask;
    }
}
