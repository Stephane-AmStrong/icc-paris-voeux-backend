using Domain.Abstractions.Events;
using Domain.Entities;

namespace Domain.Events;

public record AlertUpdatedEvent(Alert Alert) : IDomainEvent;
