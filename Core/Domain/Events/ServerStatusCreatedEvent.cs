using Domain.Abstractions.Events;
using Domain.Entities;

namespace Domain.Events;

public record ServerStatusCreatedEvent(ServerStatus ServerStatus) : IDomainEvent;
