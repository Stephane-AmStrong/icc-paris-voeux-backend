using Domain.Abstractions.Events;
using Domain.Entities;

namespace Domain.Events;

public record WishDeletedEvent(Wish Wish) : IDomainEvent;