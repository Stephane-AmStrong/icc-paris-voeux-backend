using Domain.Abstractions.Events;
using Domain.Entities;

namespace Domain.Events;

public record ServerCreatedEvent(Server Server) : IDomainEvent;
