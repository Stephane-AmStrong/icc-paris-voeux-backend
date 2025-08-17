Parfait ! Simplifions en enlevant la complexité de publication d'événement. Voici le code nettoyé :

## **🚀 CODE SIMPLIFIÉ - SANS PUBLICATION D'ÉVÉNEMENT**

---

### **1. Interface Domain (Simple)**
````csharp
namespace Domain.Abstractions.Services;

public interface IPulseTimeoutManager
{
    void ScheduleTimeout(string pulseId, string userId);
    void CancelTimeout(string pulseId);
}
````

---

### **2. Modèle de Commande**
````csharp
namespace Infrastructure.Models;

internal sealed record PulseTimeoutCommand(
    string PulseId,
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

internal sealed class PulseTimeoutBackgroundService : BackgroundService
{
    private readonly Channel<PulseTimeoutCommand> _commandChannel;
    private readonly ILogger<PulseTimeoutBackgroundService> _logger;

    private static readonly TimeSpan PulseTimeout = TimeSpan.FromSeconds(30);

    public PulseTimeoutBackgroundService(ILogger<PulseTimeoutBackgroundService> logger)
    {
        _logger = logger;

        var options = new BoundedChannelOptions(1000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false
        };
        _commandChannel = Channel.CreateBounded<PulseTimeoutCommand>(options);
    }

    public ChannelWriter<PulseTimeoutCommand> Writer => _commandChannel.Writer;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Pulse Timeout Background Service started");

        try
        {
            await foreach (var command in _commandChannel.Reader.ReadAllAsync(stoppingToken))
            {
                _ = ProcessTimeoutCommand(command, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Pulse Timeout Background Service is stopping");
        }
        finally
        {
            _logger.LogInformation("Pulse Timeout Background Service stopped");
        }
    }

    private async Task ProcessTimeoutCommand(PulseTimeoutCommand command, CancellationToken stoppingToken)
    {
        try
        {
            var combinedToken = CancellationTokenSource.CreateLinkedTokenSource(
                stoppingToken, 
                command.CancellationTokenSource.Token).Token;

            await Task.Delay(PulseTimeout, combinedToken);

            if (!combinedToken.IsCancellationRequested)
            {
                PrintTimeout(command);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogDebug("Pulse timeout cancelled for {PulseId}", command.PulseId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing pulse timeout for {PulseId}", command.PulseId);
        }
        finally
        {
            command.CancellationTokenSource.Dispose();
        }
    }

    private void PrintTimeout(PulseTimeoutCommand command)
    {
        Console.WriteLine($"[PULSE TIMEOUT] Pulse {command.PulseId} for User {command.UserId} has expired after 30 seconds!");
        Console.WriteLine($"   Scheduled at: {command.ScheduledAt:HH:mm:ss}");
        Console.WriteLine($"   Expired at: {DateTime.UtcNow:HH:mm:ss}");
        Console.WriteLine($"   Duration: {(DateTime.UtcNow - command.ScheduledAt).TotalSeconds:F1}s");
        Console.WriteLine($"   Action: Timeout handler executed");
        Console.WriteLine();

        _logger.LogInformation("Pulse timeout expired for {PulseId}", command.PulseId);
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

internal sealed class PulseTimeoutManager : IPulseTimeoutManager
{
    private readonly ConcurrentDictionary<string, CancellationTokenSource> _activeTimeouts = new();
    private readonly ChannelWriter<PulseTimeoutCommand> _commandWriter;
    private readonly ILogger<PulseTimeoutManager> _logger;

    public PulseTimeoutManager(
        PulseTimeoutBackgroundService backgroundService,
        ILogger<PulseTimeoutManager> logger)
    {
        _commandWriter = backgroundService.Writer;
        _logger = logger;
    }

    public void ScheduleTimeout(string pulseId, string userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pulseId);
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        // Annule le timeout existant s'il y en a un
        if (_activeTimeouts.TryRemove(pulseId, out var existingToken))
        {
            existingToken.Cancel();
            existingToken.Dispose();
            _logger.LogDebug("Cancelled existing timeout for pulse {PulseId}", pulseId);
        }

        // Crée un nouveau timeout
        var cancellationTokenSource = new CancellationTokenSource();
        _activeTimeouts.TryAdd(pulseId, cancellationTokenSource);

        var command = new PulseTimeoutCommand(
            pulseId,
            userId,
            DateTime.UtcNow,
            cancellationTokenSource);

        if (_commandWriter.TryWrite(command))
        {
            _logger.LogDebug("Scheduled timeout for pulse {PulseId}", pulseId);
        }
        else
        {
            _logger.LogWarning("Failed to schedule timeout for pulse {PulseId} - channel full", pulseId);
            cancellationTokenSource.Dispose();
            _activeTimeouts.TryRemove(pulseId, out _);
        }
    }

    public void CancelTimeout(string pulseId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pulseId);

        if (_activeTimeouts.TryRemove(pulseId, out var cancellationTokenSource))
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            _logger.LogDebug("Cancelled timeout for pulse {PulseId}", pulseId);
        }
    }
}
````

---

### **5. Event Handler pour PulseCreated**
````csharp
using Domain.Abstractions.Events;
using Domain.Abstractions.Services;
using Domain.Events;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Pulses.Create;

public sealed class PulseCreatedEventHandler : IEventHandler<PulseCreatedEvent>
{
    private readonly IPulseTimeoutManager _pulseTimeoutManager;
    private readonly ILogger<PulseCreatedEventHandler> _logger;

    public PulseCreatedEventHandler(
        IPulseTimeoutManager pulseTimeoutManager,
        ILogger<PulseCreatedEventHandler> logger)
    {
        _pulseTimeoutManager = pulseTimeoutManager;
        _logger = logger;
    }

    public Task Handle(PulseCreatedEvent domainEvent, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Processing pulse created event for {PulseId}", domainEvent.Pulse.Id);

        _pulseTimeoutManager.ScheduleTimeout(domainEvent.Pulse.Id, domainEvent.Pulse.UserId);

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
    services.AddSingleton<PulseTimeoutBackgroundService>();
    services.AddHostedService<PulseTimeoutBackgroundService>(provider =>
        provider.GetRequiredService<PulseTimeoutBackgroundService>());

    // Manager
    services.AddSingleton<IPulseTimeoutManager, PulseTimeoutManager>();

    // Event Handler
    services.AddScoped<IEventHandler<PulseCreatedEvent>, PulseCreatedEventHandler>();

    return services;
}
````

---

## **✅ SIMPLIFICATIONS APPORTÉES**

1. **❌ Supprimé** : `PulseTimeoutExpiredEvent`
2. **❌ Supprimé** : `PulseTimeoutExpiredEventHandler`
3. **❌ Supprimé** : `IServiceScopeFactory` et `IEventPublisher`
4. **❌ Supprimé** : `PublishTimeoutExpiredEvent()`
5. **✅ Gardé** : `PrintTimeout()` simple et direct

## **🎯 FONCTIONNEMENT**

1. **PulseCreatedEvent** → `ScheduleTimeout()`
2. **Channel** reçoit la commande
3. **Background Service** lance `Task.Delay(30s)`
4. **Après 30s** → `PrintTimeout()` affiche dans la console
5. **Fini** → Task se termine et se nettoie

**Code simple, efficace et sans complexité inutile ! 🚀**