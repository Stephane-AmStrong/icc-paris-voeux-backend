Parfait ! Simplifions en enlevant la complexité de publication d'événement. Voici le code nettoyé :

## **🚀 CODE SIMPLIFIÉ - SANS PUBLICATION D'ÉVÉNEMENT**

---

### **1. Interface Domain (Simple)**
````csharp
namespace Domain.Abstractions.Services;

public interface IHeartbeatTimeoutManager
{
    void ScheduleTimeout(string heartbeatId, string userId);
    void CancelTimeout(string heartbeatId);
}
````

---

### **2. Modèle de Commande**
````csharp
namespace Infrastructure.Models;

internal sealed record HeartbeatCommand(
    string HeartbeatId,
    string UserId,
    DateTime ScheduledAt,
    CancellationTokenSource CancellationTokenSource);
````

---

### **3. Background Service (Simplifié)**
````csharp
using System.Threading.Channels;
using Infrastructure.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackgroundServices;

internal sealed class HeartbeatBackgroundService : BackgroundService
{
    private readonly Channel<HeartbeatCommand> _commandChannel;
    private readonly ILogger<HeartbeatBackgroundService> _logger;

    private static readonly TimeSpan HeartbeatTimeout = TimeSpan.FromSeconds(30);

    public HeartbeatBackgroundService(ILogger<HeartbeatBackgroundService> logger)
    {
        _logger = logger;

        var options = new BoundedChannelOptions(1000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false
        };
        _commandChannel = Channel.CreateBounded<HeartbeatCommand>(options);
    }

    public ChannelWriter<HeartbeatCommand> Writer => _commandChannel.Writer;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Heartbeat Timeout Background Service started");

        try
        {
            await foreach (var command in _commandChannel.Reader.ReadAllAsync(stoppingToken))
            {
                _ = ProcessTimeoutCommand(command, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Heartbeat Timeout Background Service is stopping");
        }
        finally
        {
            _logger.LogInformation("Heartbeat Timeout Background Service stopped");
        }
    }

    private async Task ProcessTimeoutCommand(HeartbeatCommand command, CancellationToken stoppingToken)
    {
        try
        {
            var combinedToken = CancellationTokenSource.CreateLinkedTokenSource(
                stoppingToken, 
                command.CancellationTokenSource.Token).Token;

            await Task.Delay(HeartbeatTimeout, combinedToken);

            if (!combinedToken.IsCancellationRequested)
            {
                PrintTimeout(command);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogDebug("Heartbeat timeout cancelled for {HeartbeatId}", command.HeartbeatId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing heartbeat timeout for {HeartbeatId}", command.HeartbeatId);
        }
        finally
        {
            command.CancellationTokenSource.Dispose();
        }
    }

    private void PrintTimeout(HeartbeatCommand command)
    {
        Console.WriteLine($"[HEARTBEAT TIMEOUT] Heartbeat {command.HeartbeatId} for User {command.UserId} has expired after 30 seconds!");
        Console.WriteLine($"   Scheduled at: {command.ScheduledAt:HH:mm:ss}");
        Console.WriteLine($"   Expired at: {DateTime.UtcNow:HH:mm:ss}");
        Console.WriteLine($"   Duration: {(DateTime.UtcNow - command.ScheduledAt).TotalSeconds:F1}s");
        Console.WriteLine($"   Action: Timeout handler executed");
        Console.WriteLine();

        _logger.LogInformation("Heartbeat timeout expired for {HeartbeatId}", command.HeartbeatId);
    }

    public override void Dispose()
    {
        _commandChannel.Writer.Complete();
        base.Dispose();
    }
}
````

---

### **4. Manager (Sans événement)**
````csharp
using System.Collections.Concurrent;
using System.Threading.Channels;
using Domain.Abstractions.Services;
using Infrastructure.BackgroundServices;
using Infrastructure.Models;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

internal sealed class HeartbeatTimeoutManager : IHeartbeatTimeoutManager
{
    private readonly ConcurrentDictionary<string, CancellationTokenSource> _activeTimeouts = new();
    private readonly ChannelWriter<HeartbeatCommand> _commandWriter;
    private readonly ILogger<HeartbeatTimeoutManager> _logger;

    public HeartbeatTimeoutManager(
        HeartbeatBackgroundService backgroundService,
        ILogger<HeartbeatTimeoutManager> logger)
    {
        _commandWriter = backgroundService.Writer;
        _logger = logger;
    }

    public void ScheduleTimeout(string heartbeatId, string userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(heartbeatId);
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        // Annule le timeout existant s'il y en a un
        if (_activeTimeouts.TryRemove(heartbeatId, out var existingToken))
        {
            existingToken.Cancel();
            existingToken.Dispose();
            _logger.LogDebug("Cancelled existing timeout for heartbeat {HeartbeatId}", heartbeatId);
        }

        // Crée un nouveau timeout
        var cancellationTokenSource = new CancellationTokenSource();
        _activeTimeouts.TryAdd(heartbeatId, cancellationTokenSource);

        var command = new HeartbeatCommand(
            heartbeatId,
            userId,
            DateTime.UtcNow,
            cancellationTokenSource);

        if (_commandWriter.TryWrite(command))
        {
            _logger.LogDebug("Scheduled timeout for heartbeat {HeartbeatId}", heartbeatId);
        }
        else
        {
            _logger.LogWarning("Failed to schedule timeout for heartbeat {HeartbeatId} - channel full", heartbeatId);
            cancellationTokenSource.Dispose();
            _activeTimeouts.TryRemove(heartbeatId, out _);
        }
    }

    public void CancelTimeout(string heartbeatId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(heartbeatId);

        if (_activeTimeouts.TryRemove(heartbeatId, out var cancellationTokenSource))
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            _logger.LogDebug("Cancelled timeout for heartbeat {HeartbeatId}", heartbeatId);
        }
    }
}
````

---

### **5. Event Handler pour HeartbeatCreated**
````csharp
using Domain.Abstractions.Events;
using Domain.Abstractions.Services;
using Domain.Events;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Heartbeats.Create;

public sealed class HeartbeatCreatedEventHandler : IEventHandler<HeartbeatCreatedEvent>
{
    private readonly IHeartbeatTimeoutManager _heartbeatTimeoutManager;
    private readonly ILogger<HeartbeatCreatedEventHandler> _logger;

    public HeartbeatCreatedEventHandler(
        IHeartbeatTimeoutManager heartbeatTimeoutManager,
        ILogger<HeartbeatCreatedEventHandler> logger)
    {
        _heartbeatTimeoutManager = heartbeatTimeoutManager;
        _logger = logger;
    }

    public Task Handle(HeartbeatCreatedEvent domainEvent, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Processing heartbeat created event for {HeartbeatId}", domainEvent.Heartbeat.Id);

        _heartbeatTimeoutManager.ScheduleTimeout(domainEvent.Heartbeat.Id, domainEvent.Heartbeat.UserId);

        return Task.CompletedTask;
    }
}
````

---

### **6. Configuration DI**
````csharp
public static IServiceCollection ConfigureServices(this IServiceCollection services)
{
    // Background Service
    services.AddSingleton<HeartbeatBackgroundService>();
    services.AddHostedService<HeartbeatBackgroundService>(provider =>
        provider.GetRequiredService<HeartbeatBackgroundService>());

    // Manager
    services.AddSingleton<IHeartbeatTimeoutManager, HeartbeatTimeoutManager>();

    // Event Handler
    services.AddScoped<IEventHandler<HeartbeatCreatedEvent>, HeartbeatCreatedEventHandler>();

    return services;
}
````

---

## **✅ SIMPLIFICATIONS APPORTÉES**

1. **❌ Supprimé** : `HeartbeatTimeoutExpiredEvent`
2. **❌ Supprimé** : `HeartbeatTimeoutExpiredEventHandler`
3. **❌ Supprimé** : `IServiceScopeFactory` et `IEventPublisher`
4. **❌ Supprimé** : `PublishTimeoutExpiredEvent()`
5. **✅ Gardé** : `PrintTimeout()` simple et direct

## **🎯 FONCTIONNEMENT**

1. **HeartbeatCreatedEvent** → `ScheduleTimeout()`
2. **Channel** reçoit la commande
3. **Background Service** lance `Task.Delay(30s)`
4. **Après 30s** → `PrintTimeout()` affiche dans la console
5. **Fini** → Task se termine et se nettoie

**Code simple, efficace et sans complexité inutile ! 🚀**