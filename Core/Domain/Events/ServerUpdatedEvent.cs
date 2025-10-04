using Domain.Abstractions.Events;
using Domain.Entities;

namespace Domain.Events;

public record ServerUpdatedEvent(Server Server) : IDomainEvent;
