using Domain.Abstractions.Events;
using Domain.Entities;

namespace Domain.Events;

public record UserDeletedEvent(User User) : IDomainEvent;