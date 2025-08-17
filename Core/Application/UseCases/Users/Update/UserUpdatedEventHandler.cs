using Domain.Abstractions.Events;
using Domain.Events;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Users.Update;

public class UserUpdatedEventHandler(ILogger<UserUpdatedEventHandler> logger) : IEventHandler<UserUpdatedEvent>
{
    public Task Handle(UserUpdatedEvent updatedEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation("User profile updated - UserId: {UserId}, Name: {FullName}", updatedEvent.User.Id, $"{updatedEvent.User.FirstName} {updatedEvent.User.LastName}");

        return Task.CompletedTask;
    }
}