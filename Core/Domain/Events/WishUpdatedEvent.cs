using Domain.Abstractions.Events;
using Domain.Entities;

namespace Domain.Events;

public record WishUpdatedEvent(Wish Wish) : IDomainEvent;