using System.Collections.Concurrent;
using System.Reflection;
using Domain.Abstractions.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Persistence.Events;

public sealed class EventsDispatcher(IServiceProvider serviceProvider, ILogger<EventsDispatcher> logger)
    : IEventsDispatcher
{
    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    private readonly ILogger<EventsDispatcher> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    // Cache of types AND methods
    private static readonly ConcurrentDictionary<Type, Type> HandlerTypeCache = new();
    private static readonly ConcurrentDictionary<Type, MethodInfo> HandleMethodCache = new();

    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(domainEvents);

        var events = domainEvents.ToList();
        if (events.Count == 0) return;

        _logger.LogDebug("Dispatching {EventCount} domain events", events.Count);

        using var scope = _serviceProvider.CreateScope();
        
        var tasks = events
            .GroupBy(e => e.GetType())
            .Select(group => DispatchEventGroup(group.Key, group.ToList(), scope.ServiceProvider, cancellationToken))
            .ToList();

        await Task.WhenAll(tasks);
    }

    private async Task DispatchEventGroup(Type eventType, List<IDomainEvent> events, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var handlerType = GetHandlerType(eventType);
        var handlers = serviceProvider.GetServices(handlerType).Where(h => h is not null).ToList();

        if (handlers.Count == 0)
        {
            _logger.LogWarning("No handlers found for event type {EventType}", eventType.Name);
            return;
        }

        _logger.LogDebug("Processing {EventCount} events of type {EventType} with {HandlerCount} handlers", 
            events.Count, eventType.Name, handlers.Count);

        // Optimization: retrieves the Handle method only once
        var handleMethod = GetHandleMethod(handlerType);

        var tasks = from eventItem in events
                   from handler in handlers
                   select ExecuteHandlerSafely(handler, eventItem, handleMethod, cancellationToken);

        await Task.WhenAll(tasks);
    }

    private static Type GetHandlerType(Type eventType)
    {
        return HandlerTypeCache.GetOrAdd(eventType, 
            static et => typeof(IEventHandler<>).MakeGenericType(et));
    }

    private static MethodInfo GetHandleMethod(Type handlerType)
    {
        return HandleMethodCache.GetOrAdd(handlerType, static ht =>
        {
            var method = ht.GetMethod("Handle", [typeof(IDomainEvent).IsAssignableFrom(ht.GenericTypeArguments[0]) ? ht.GenericTypeArguments[0] : typeof(IDomainEvent), typeof(CancellationToken)]);
            return method ?? throw new InvalidOperationException($"Handle method not found on {ht.Name}");
        });
    }

    private async Task ExecuteHandlerSafely(object handler, IDomainEvent domainEvent, MethodInfo handleMethod, CancellationToken cancellationToken)
    {
        try
        {
            if (handleMethod.Invoke(handler, [domainEvent, cancellationToken]) is Task task)
            {
                await task;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handler {HandlerType} failed for event {EventType} - continuing with other handlers", handler.GetType().Name, domainEvent.GetType().Name);
        }
    }
}