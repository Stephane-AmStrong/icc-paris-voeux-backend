using Domain.Abstractions.Events;
using Domain.Events;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Users.Delete;

public class UserDeletedEventHandler(ILogger<UserDeletedEventHandler> logger) : IEventHandler<UserDeletedEvent>
{
    public Task Handle(UserDeletedEvent deletedEvent, CancellationToken cancellationToken)
    {
        logger.LogWarning("User account deleted - UserId: {UserId}", deletedEvent.UserId);

        return Task.CompletedTask;
    }
}