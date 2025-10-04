using Domain.Abstractions.Events;
using Domain.Entities;

namespace Domain.Events;

public record AlertCreatedEvent(Alert Alert) : IDomainEvent;
