namespace Domain.Abstractions.Events;

public interface IDomainEvent;

public interface IEventHandler<in T> where T : IDomainEvent
{
    Task Handle(T domainEvent, CancellationToken cancellationToken);
}