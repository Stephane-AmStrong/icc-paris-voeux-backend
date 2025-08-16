using System.Collections.Concurrent;
using Domain.Abstractions.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Persistence.Events;

public sealed class EventsDispatcher(IServiceProvider serviceProvider, ILogger<EventsDispatcher> logger)
    : IEventsDispatcher
{
    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    private readonly ILogger<EventsDispatcher> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(domainEvents);

        var events = domainEvents.ToList();
        if (events.Count == 0) return;

        _logger.LogDebug("Dispatching {EventCount} domain events", events.Count);

        // Group events by type and process them in parallel
        var tasks = events
            .GroupBy(e => e.GetType())
            .Select(group => DispatchEventGroup(group.Key, group.ToList(), cancellationToken));

        await Task.WhenAll(tasks);
    }

    private async Task DispatchEventGroup(Type eventType, List<IDomainEvent> events, CancellationToken cancellationToken)
    {
        var handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);

        using var scope = _serviceProvider.CreateScope();
        var handlers = scope.ServiceProvider.GetServices(handlerType).Where(h => h is not null).ToList();

        if (handlers.Count == 0)
        {
            _logger.LogWarning("No handlers found for event type {EventType}", eventType.Name);
            return;
        }

        _logger.LogDebug("Processing {EventCount} events of type {EventType} with {HandlerCount} handlers", events.Count, eventType.Name, handlers.Count);

        // Exécute tous les handlers pour tous les événements de ce type
        var tasks = from eventItem in events
                   from handler in handlers
                   select ExecuteHandlerSafely(handler!, eventItem, cancellationToken);

        await Task.WhenAll(tasks);
    }

    private async Task ExecuteHandlerSafely(object handler, IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        try
        {
            var handleMethod = handler.GetType().GetMethod("Handle") 
                ?? throw new InvalidOperationException($"Handle method not found on {handler.GetType().Name}");

            if (handleMethod.Invoke(handler, [domainEvent, cancellationToken]) is Task task)
            {
                await task;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Handler {HandlerType} failed to process event {EventType}", 
                handler.GetType().Name, 
                domainEvent.GetType().Name);
            throw;
        }
    }
}