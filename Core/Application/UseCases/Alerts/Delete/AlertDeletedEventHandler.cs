using Domain.Abstractions.Events;
using Domain.Events;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Alerts.Delete;

public class AlertDeletedEventHandler(ILogger<AlertDeletedEventHandler> logger) : IEventHandler<AlertDeletedEvent>
{
    public Task Handle(AlertDeletedEvent deletedEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation("Alert successfully deleted - Id: {AlertId}, ServerId: {ServerId}, OccurredAt: {OccurredAt}", deletedEvent.Alert.Id, deletedEvent.Alert.ServerId, deletedEvent.Alert.OccurredAt);

        logger.LogDebug("Alert deletion event processed for {AlertId} at {ProcessedAt}", deletedEvent.Alert.Id, DateTime.UtcNow);

        return Task.CompletedTask;
    }
}
