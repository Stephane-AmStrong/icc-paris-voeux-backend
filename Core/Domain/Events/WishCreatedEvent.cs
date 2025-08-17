using Domain.Abstractions.Events;
using Domain.Entities;

namespace Domain.Events;

public record WishCreatedEvent(Wish Wish) : IDomainEvent;