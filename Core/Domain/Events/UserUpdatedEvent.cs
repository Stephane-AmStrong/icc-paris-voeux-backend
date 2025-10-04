using Domain.Abstractions.Events;
using Domain.Entities;

namespace Domain.Events;

public record UserUpdatedEvent(User User) : IDomainEvent;