using Domain.Abstractions.Events;
using Domain.Entities;

namespace Domain.Events;

public record UserCreatedEvent(User User) : IDomainEvent;