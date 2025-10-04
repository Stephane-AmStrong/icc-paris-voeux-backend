using Domain.Abstractions.Events;
using Domain.Entities;

namespace Domain.Events;

public record AlertDeletedEvent(Alert Alert) : IDomainEvent;
