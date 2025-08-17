using Domain.Abstractions.Events;

namespace Domain.Events;

public record UserDeletedEvent(string UserId) : IDomainEvent;