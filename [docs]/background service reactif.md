Excellente question ! Voici comment mettre en place un **Background Service** qui réagit aux événements domaine, étape par étape.

## **📋 PLAN DÉTAILLÉ - BACKGROUND SERVICE RÉACTIF**

---

## **🏗️ ÉTAPE 1 : Interface pour le Background Service**

````csharp
namespace Domain.Abstractions.Services;

public interface IEmailService
{
    Task SendWelcomeEmailAsync(string userId, string email, string userName, CancellationToken cancellationToken = default);
    Task SendVerificationEmailAsync(string userId, string email, string verificationToken, CancellationToken cancellationToken = default);
}
````

---

## **🏗️ ÉTAPE 2 : Event Handler pour Background Task**

### **2.1 Nouveau Handler pour Email**
````csharp
using Domain.Abstractions.Events;
using Domain.Abstractions.Services;
using Domain.Events;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Users.Create;

public class UserCreatedEmailHandler : IEventHandler<UserCreatedEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<UserCreatedEmailHandler> _logger;

    public UserCreatedEmailHandler(
        IEmailService emailService,
        ILogger<UserCreatedEmailHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Handle(UserCreatedEvent domainEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[UserCreatedEmailHandler] Processing email for user {UserId}", domainEvent.User.Id);

        try
        {
            // Envoie l'email de bienvenue
            await _emailService.SendWelcomeEmailAsync(
                domainEvent.User.Id.ToString(),
                domainEvent.User.Email,
                domainEvent.User.Name,
                cancellationToken);

            _logger.LogInformation("[UserCreatedEmailHandler] Welcome email sent to {Email}", domainEvent.User.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[UserCreatedEmailHandler] Failed to send welcome email to {Email}", domainEvent.User.Email);
            // Ne pas re-throw pour ne pas casser les autres handlers
        }
    }
}
````

---

## **🏗️ ÉTAPE 3 : Background Service Implementation**

### **3.1 Email Background Service**
````csharp
using System.Threading.Channels;
using Domain.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class EmailBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EmailBackgroundService> _logger;
    private readonly Channel<EmailTask> _emailQueue;

    public EmailBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<EmailBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        
        // Canal pour la queue des emails (capacity: 1000)
        var options = new BoundedChannelOptions(1000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false
        };
        _emailQueue = Channel.CreateBounded<EmailTask>(options);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Email Background Service started");

        await foreach (var emailTask in _emailQueue.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                await ProcessEmailTask(emailTask, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing email task: {TaskType}", emailTask.TaskType);
            }
        }

        _logger.LogInformation("Email Background Service stopped");
    }

    public async Task<bool> QueueWelcomeEmailAsync(string userId, string email, string userName)
    {
        var emailTask = new EmailTask
        {
            TaskType = EmailTaskType.Welcome,
            UserId = userId,
            Email = email,
            UserName = userName,
            CreatedAt = DateTime.UtcNow
        };

        return await _emailQueue.Writer.TryWriteAsync(emailTask);
    }

    public async Task<bool> QueueVerificationEmailAsync(string userId, string email, string verificationToken)
    {
        var emailTask = new EmailTask
        {
            TaskType = EmailTaskType.Verification,
            UserId = userId,
            Email = email,
            VerificationToken = verificationToken,
            CreatedAt = DateTime.UtcNow
        };

        return await _emailQueue.Writer.TryWriteAsync(emailTask);
    }

    private async Task ProcessEmailTask(EmailTask emailTask, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        _logger.LogDebug("Processing email task: {TaskType} for {Email}", emailTask.TaskType, emailTask.Email);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            switch (emailTask.TaskType)
            {
                case EmailTaskType.Welcome:
                    await emailService.SendWelcomeEmailAsync(emailTask.UserId!, emailTask.Email!, emailTask.UserName!, cancellationToken);
                    break;
                
                case EmailTaskType.Verification:
                    await emailService.SendVerificationEmailAsync(emailTask.UserId!, emailTask.Email!, emailTask.VerificationToken!, cancellationToken);
                    break;
                
                default:
                    _logger.LogWarning("Unknown email task type: {TaskType}", emailTask.TaskType);
                    return;
            }

            stopwatch.Stop();
            _logger.LogInformation("Email task {TaskType} completed for {Email} in {ElapsedMs}ms", 
                emailTask.TaskType, emailTask.Email, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Email task {TaskType} failed for {Email} after {ElapsedMs}ms", 
                emailTask.TaskType, emailTask.Email, stopwatch.ElapsedMilliseconds);
            
            // TODO: Implémenter retry logic ou dead letter queue
        }
    }

    public override void Dispose()
    {
        _emailQueue.Writer.Complete();
        base.Dispose();
    }
}

public class EmailTask
{
    public EmailTaskType TaskType { get; set; }
    public string? UserId { get; set; }
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public string? VerificationToken { get; set; }
    public DateTime CreatedAt { get; set; }
}

public enum EmailTaskType
{
    Welcome,
    Verification
}
````

---

## **🏗️ ÉTAPE 4 : Service Email Concret**

### **4.1 Implémentation Email Service**
````csharp
using Domain.Abstractions.Services;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly EmailBackgroundService _backgroundService;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        EmailBackgroundService backgroundService,
        ILogger<EmailService> logger)
    {
        _backgroundService = backgroundService;
        _logger = logger;
    }

    public async Task SendWelcomeEmailAsync(string userId, string email, string userName, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Queueing welcome email for {Email}", email);

        var queued = await _backgroundService.QueueWelcomeEmailAsync(userId, email, userName);
        
        if (!queued)
        {
            _logger.LogWarning("Failed to queue welcome email for {Email} - queue may be full", email);
            throw new InvalidOperationException($"Failed to queue welcome email for {email}");
        }

        _logger.LogDebug("Welcome email queued successfully for {Email}", email);
    }

    public async Task SendVerificationEmailAsync(string userId, string email, string verificationToken, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Queueing verification email for {Email}", email);

        var queued = await _backgroundService.QueueVerificationEmailAsync(userId, email, verificationToken);
        
        if (!queued)
        {
            _logger.LogWarning("Failed to queue verification email for {Email} - queue may be full", email);
            throw new InvalidOperationException($"Failed to queue verification email for {email}");
        }

        _logger.LogDebug("Verification email queued successfully for {Email}", email);
    }
}
````

---

## **🏗️ ÉTAPE 5 : Mise à jour du Handler existant**

### **5.1 UserCreatedEventHandler mis à jour**
````csharp
using Domain.Abstractions.Events;
using Domain.Abstractions.Services;
using Domain.Events;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Users.Create;

public class UserCreatedEventHandler : IEventHandler<UserCreatedEvent>
{
    private readonly ISseNotificationService? _sseService;
    private readonly ILogger<UserCreatedEventHandler> _logger;

    public UserCreatedEventHandler(
        ILogger<UserCreatedEventHandler> logger,
        ISseNotificationService? sseService = null) // Optionnel
    {
        _logger = logger;
        _sseService = sseService;
    }

    public async Task Handle(UserCreatedEvent domainEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[UserCreatedEventHandler] User created with ID: {UserId}", domainEvent.User.Id);

        // ✅ SSE Notification (si le service est disponible)
        if (_sseService != null)
        {
            try
            {
                var notification = new
                {
                    UserId = domainEvent.User.Id.ToString(),
                    UserName = domainEvent.User.Name,
                    Email = domainEvent.User.Email,
                    CreatedAt = domainEvent.User.CreatedAt,
                    Message = $"New user '{domainEvent.User.Name}' has been created"
                };

                await _sseService.BroadcastAsync("user.created", notification, cancellationToken);
                _logger.LogDebug("[UserCreatedEventHandler] SSE notification sent for user {UserId}", domainEvent.User.Id);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[UserCreatedEventHandler] Failed to send SSE notification for user {UserId}", domainEvent.User.Id);
            }
        }

        _logger.LogDebug("[UserCreatedEventHandler] Processing completed for user {UserId}", domainEvent.User.Id);
    }
}
````

---

## **🏗️ ÉTAPE 6 : Configuration DI**

### **6.1 ServiceCollectionExtensions mis à jour**
````csharp
// Dans ServiceCollectionExtensions.cs
public static IServiceCollection ConfigureServices(this IServiceCollection services)
{
    // ... existing services ...
    
    // ✅ Background Services
    services.AddSingleton<EmailBackgroundService>();
    services.AddHostedService<EmailBackgroundService>(provider => 
        provider.GetRequiredService<EmailBackgroundService>());
    
    // ✅ Email Services
    services.AddScoped<IEmailService, EmailService>();
    
    // ✅ SSE Service (optionnel)
    services.AddSingleton<SseNotificationService>();
    services.AddSingleton<ISseNotificationService>(provider => 
        provider.GetRequiredService<SseNotificationService>());
    
    return services;
}

public static void ConfigureValidation(this IServiceCollection services)
{
    // ... existing validators ...
    
    // ✅ Ajoute le nouveau handler pour les emails
    services.AddScoped<IEventHandler<UserCreatedEvent>, UserCreatedEmailHandler>();
}
````

---

## **🏗️ ÉTAPE 7 : Mock Email Service pour Dev**

### **7.1 Service d'email simulé**
````csharp
using Domain.Abstractions.Services;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

// Service temporaire pour simulation
public class MockEmailService : IEmailService
{
    private readonly ILogger<MockEmailService> _logger;

    public MockEmailService(ILogger<MockEmailService> logger)
    {
        _logger = logger;
    }

    public async Task SendWelcomeEmailAsync(string userId, string email, string userName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("📧 [MOCK] Sending welcome email to {Email} for user {UserName}", email, userName);
        
        // Simule un délai d'envoi d'email
        await Task.Delay(500, cancellationToken);
        
        _logger.LogInformation("✅ [MOCK] Welcome email sent to {Email}", email);
    }

    public async Task SendVerificationEmailAsync(string userId, string email, string verificationToken, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("📧 [MOCK] Sending verification email to {Email} with token {Token}", email, verificationToken);
        
        // Simule un délai d'envoi d'email
        await Task.Delay(700, cancellationToken);
        
        _logger.LogInformation("✅ [MOCK] Verification email sent to {Email}", email);
    }
}
````

### **7.2 Configuration conditionnelle**
````csharp
// Dans ServiceCollectionExtensions.cs
public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
{
    // ... existing services ...
    
    // ✅ Background Services
    services.AddSingleton<EmailBackgroundService>();
    services.AddHostedService<EmailBackgroundService>(provider => 
        provider.GetRequiredService<EmailBackgroundService>());
    
    // ✅ Email Service conditionnel
    if (configuration.GetValue<bool>("UseRealEmailService"))
    {
        services.AddScoped<IEmailService, EmailService>();
    }
    else
    {
        services.AddScoped<IEmailService, MockEmailService>();
    }
    
    return services;
}
````

---

## **🎯 ARCHITECTURE COMPLÈTE**

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   UserCreated   │    │ EventDispatcher │    │ EventHandlers   │
│     Event       │───▶│  (Parallel)     │───▶│   (Multiple)    │
└─────────────────┘    └─────────────────┘    └─────────────────┘
                                                       │
                       ┌────────────────────────────────┼────────────────────┐
                       │                                │                    │
                       ▼                                ▼                    ▼
            ┌─────────────────┐              ┌─────────────────┐   ┌─────────────────┐
            │ SSE Handler     │              │ Email Handler   │   │ Other Handler   │
            │ (Immediate)     │              │ (Queue)         │   │ (Future...)     │
            └─────────────────┘              └─────────────────┘   └─────────────────┘
                       │                                │
                       ▼                                ▼
            ┌─────────────────┐              ┌─────────────────┐
            │ SSE Broadcast   │              │ Background      │
            │ to All Clients  │              │ Email Service   │
            └─────────────────┘              └─────────────────┘
```

## **✅ RÉSULTAT FINAL**

**Quand un utilisateur est créé :**

1. **`UserCreatedEvent`** est dispatché
2. **Parallèlement :**
   - **`UserCreatedEventHandler`** → Envoie notification SSE
   - **`UserCreatedEmailHandler`** → Queue l'email en background
3. **`EmailBackgroundService`** → Traite les emails de façon asynchrone

**Avantages :**
- ✅ **Non-bloquant** : L'API répond immédiatement
- ✅ **Résilient** : Si l'email échoue, l'API fonctionne toujours
- ✅ **Scalable** : Queue gérable avec retry logic
- ✅ **Séparé** : Chaque handler a sa responsabilité

**Ton background service réagit maintenant aux événements domaine ! 🚀**