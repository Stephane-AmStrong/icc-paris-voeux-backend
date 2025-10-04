using Domain.Abstractions.Events;
using Domain.Entities;

namespace Domain.Events;

public record ServerDeletedEvent(Server Server) : IDomainEvent;
