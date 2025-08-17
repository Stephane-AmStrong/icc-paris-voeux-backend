using Domain.Abstractions.Events;
using Domain.Events;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Users.Create;

public class UserCreatedEventHandler(ILogger<UserCreatedEventHandler> logger) : IEventHandler<UserCreatedEvent>
{
    public Task Handle(UserCreatedEvent createdEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation("New user successfully created - UserId: {UserId}, Name: {FullName}", createdEvent.User.Id, $"{createdEvent.User.FirstName} {createdEvent.User.LastName}");

        logger.LogDebug("User creation event processed for {UserId} at {ProcessedAt}", createdEvent.User.Id, DateTime.UtcNow);

        return Task.CompletedTask;
    }
}