# Clean Onion Architecture - Structure par Projets

```
MyProject.sln
│
├── 📁 1-Domain/                        # 🎯 COUCHE DOMAINE (Cœur)
│   └── 📦 MyProject.Domain/            # Projet principal du domaine
│       ├── Entities/                   # Entités métier
│       │   ├── User.cs
│       │   ├── Product.cs
│       │   └── Order.cs
│       ├── ValueObjects/               # Objets de valeur
│       │   ├── Email.cs
│       │   ├── Money.cs
│       │   └── Address.cs
│       ├── Interfaces/                 # Interfaces des repositories
│       │   ├── IUserRepository.cs
│       │   ├── IProductRepository.cs
│       │   └── IOrderRepository.cs
│       ├── Services/                   # Services du domaine
│       │   ├── IDomainEventService.cs
│       │   └── IPasswordHashingService.cs
│       ├── Events/                     # Événements du domaine
│       │   ├── UserCreatedEvent.cs
│       │   └── OrderCompletedEvent.cs
│       ├── Exceptions/                 # Exceptions métier
│       │   ├── UserNotFoundException.cs
│       │   └── InvalidEmailException.cs
│       └── MyProject.Domain.csproj
│
├── 📁 2-Application/                   # 📋 COUCHE APPLICATION
│   ├── 📦 MyProject.Application.UseCases/   # Projet des cas d'usage
│   │   ├── Users/
│   │   │   ├── CreateUser/
│   │   │   │   ├── CreateUserCommand.cs
│   │   │   │   ├── CreateUserHandler.cs
│   │   │   │   └── CreateUserValidator.cs
│   │   │   ├── GetUser/
│   │   │   │   ├── GetUserQuery.cs
│   │   │   │   └── GetUserHandler.cs
│   │   │   └── UpdateUser/
│   │   │       ├── UpdateUserCommand.cs
│   │   │       └── UpdateUserHandler.cs
│   │   ├── Products/
│   │   │   ├── CreateProduct/
│   │   │   ├── GetProducts/
│   │   │   └── UpdateProduct/
│   │   ├── Orders/
│   │   │   ├── CreateOrder/
│   │   │   ├── GetOrders/
│   │   │   └── CompleteOrder/
│   │   ├── Common/
│   │   │   ├── Behaviors/              # Behaviors pour MediatR
│   │   │   │   ├── ValidationBehavior.cs
│   │   │   │   └── LoggingBehavior.cs
│   │   │   └── Models/
│   │   │       ├── PaginatedResult.cs
│   │   │       └── Result.cs
│   │   └── MyProject.Application.UseCases.csproj
│   │
│   ├── 📦 MyProject.Application.DTOs/       # Projet des DTOs
│   │   ├── Users/
│   │   │   ├── UserDto.cs
│   │   │   ├── CreateUserDto.cs
│   │   │   └── UpdateUserDto.cs
│   │   ├── Products/
│   │   │   ├── ProductDto.cs
│   │   │   ├── CreateProductDto.cs
│   │   │   └── UpdateProductDto.cs
│   │   ├── Orders/
│   │   │   ├── OrderDto.cs
│   │   │   └── CreateOrderDto.cs
│   │   └── MyProject.Application.DTOs.csproj
│   │
│   ├── 📦 MyProject.Application.Interfaces/ # Projet des interfaces
│   │   ├── Services/
│   │   │   ├── IEmailService.cs
│   │   │   ├── IFileStorageService.cs
│   │   │   └── INotificationService.cs
│   │   ├── External/
│   │   │   ├── IPaymentGateway.cs
│   │   │   └── IThirdPartyService.cs
│   │   └── MyProject.Application.Interfaces.csproj
│   │
│   └── 📦 MyProject.Application.Mappings/   # Projet des mappeurs
│       ├── Profiles/
│       │   ├── UserProfile.cs
│       │   ├── ProductProfile.cs
│       │   └── OrderProfile.cs
│       ├── Extensions/
│       │   └── MappingExtensions.cs
│       └── MyProject.Application.Mappings.csproj
│
├── 📁 3-Infrastructure/                # 🔧 COUCHE INFRASTRUCTURE
│   ├── 📦 MyProject.Infrastructure.Data/    # Projet d'accès aux données
│   │   ├── Context/
│   │   │   └── ApplicationDbContext.cs
│   │   ├── Configurations/             # Configurations Entity Framework
│   │   │   ├── UserConfiguration.cs
│   │   │   ├── ProductConfiguration.cs
│   │   │   └── OrderConfiguration.cs
│   │   ├── Repositories/               # Implémentations des repositories
│   │   │   ├── UserRepository.cs
│   │   │   ├── ProductRepository.cs
│   │   │   └── OrderRepository.cs
│   │   ├── Migrations/                 # Migrations de base de données
│   │   ├── Seed/                       # Données de test
│   │   │   └── DataSeeder.cs
│   │   └── MyProject.Infrastructure.Data.csproj
│   │
│   ├── 📦 MyProject.Infrastructure.Services/ # Projet des services
│   │   ├── Email/
│   │   │   ├── EmailService.cs
│   │   │   └── EmailTemplateService.cs
│   │   ├── Storage/
│   │   │   ├── FileStorageService.cs
│   │   │   └── BlobStorageService.cs
│   │   ├── Notifications/
│   │   │   ├── NotificationService.cs
│   │   │   └── PushNotificationService.cs
│   │   ├── Security/
│   │   │   ├── PasswordHashingService.cs
│   │   │   └── TokenService.cs
│   │   └── MyProject.Infrastructure.Services.csproj
│   │
│   ├── 📦 MyProject.Infrastructure.ExternalAPIs/ # Intégrations externes
│   │   ├── PaymentGateways/
│   │   │   ├── StripePaymentGateway.cs
│   │   │   └── PayPalPaymentGateway.cs
│   │   ├── ThirdPartyServices/
│   │   │   ├── GoogleMapsService.cs
│   │   │   └── SendGridEmailService.cs
│   │   └── MyProject.Infrastructure.ExternalAPIs.csproj
│   │
│   └── 📦 MyProject.Infrastructure.Common/   # Configuration commune
│       ├── Logging/
│       │   ├── LoggingExtensions.cs
│       │   └── ApplicationLogger.cs
│       ├── Configuration/
│       │   ├── DatabaseConfiguration.cs
│       │   ├── ServiceConfiguration.cs
│       │   └── DependencyInjection.cs
│       └── MyProject.Infrastructure.Common.csproj
│
├── 📁 4-Presentation/                  # 🖥️ COUCHE PRÉSENTATION
│   ├── 📦 MyProject.WebAPI/            # API REST
│   │   ├── Controllers/
│   │   │   ├── UsersController.cs
│   │   │   ├── ProductsController.cs
│   │   │   └── OrdersController.cs
│   │   ├── Middlewares/                # Middlewares personnalisés
│   │   │   ├── ExceptionMiddleware.cs
│   │   │   ├── AuthenticationMiddleware.cs
│   │   │   └── LoggingMiddleware.cs
│   │   ├── Filters/                    # Filtres d'action
│   │   │   ├── ValidationFilter.cs
│   │   │   └── AuthorizationFilter.cs
│   │   ├── Models/                     # ViewModels / Request/Response
│   │   │   ├── Requests/
│   │   │   │   ├── CreateUserRequest.cs
│   │   │   │   └── UpdateUserRequest.cs
│   │   │   └── Responses/
│   │   │       ├── UserResponse.cs
│   │   │       └── ApiResponse.cs
│   │   ├── Configuration/
│   │   │   ├── SwaggerConfiguration.cs
│   │   │   └── CorsConfiguration.cs
│   │   ├── Program.cs
│   │   ├── Startup.cs
│   │   └── MyProject.WebAPI.csproj
│   │
│   ├── 📦 MyProject.MVC/               # Interface web (optionnel)
│   │   ├── Controllers/
│   │   ├── Views/
│   │   ├── Models/
│   │   ├── wwwroot/
│   │   └── MyProject.MVC.csproj
│   │
│   └── 📦 MyProject.Console/           # Application console (optionnel)
│       ├── Commands/
│       ├── Services/
│       ├── Program.cs
│       └── MyProject.Console.csproj
│
├── 📁 5-Shared/                        # 🔄 ÉLÉMENTS PARTAGÉS
│   ├── 📦 MyProject.Shared.Constants/  # Constantes globales
│   │   ├── ApplicationConstants.cs
│   │   ├── ErrorMessages.cs
│   │   └── MyProject.Shared.Constants.csproj
│   │
│   ├── 📦 MyProject.Shared.Extensions/ # Extensions methods
│   │   ├── StringExtensions.cs
│   │   ├── DateTimeExtensions.cs
│   │   ├── CollectionExtensions.cs
│   │   └── MyProject.Shared.Extensions.csproj
│   │
│   ├── 📦 MyProject.Shared.Helpers/    # Classes utilitaires
│   │   ├── ValidationHelper.cs
│   │   ├── CryptographyHelper.cs
│   │   └── MyProject.Shared.Helpers.csproj
│   │
│   └── 📦 MyProject.Shared.Enums/      # Énumérations
│       ├── UserRole.cs
│       ├── OrderStatus.cs
│       ├── ProductCategory.cs
│       └── MyProject.Shared.Enums.csproj
│
└── 📁 Tests/                           # 🧪 TESTS
    ├── 📦 MyProject.Domain.Tests/      # Tests du domaine
    │   ├── Entities/
    │   ├── ValueObjects/
    │   ├── Services/
    │   └── MyProject.Domain.Tests.csproj
    │
    ├── 📦 MyProject.Application.Tests/ # Tests de l'application
    │   ├── UseCases/
    │   ├── Services/
    │   ├── Mappings/
    │   └── MyProject.Application.Tests.csproj
    │
    ├── 📦 MyProject.Infrastructure.Tests/ # Tests de l'infrastructure
    │   ├── Repositories/
    │   ├── Services/
    │   ├── ExternalAPIs/
    │   └── MyProject.Infrastructure.Tests.csproj
    │
    ├── 📦 MyProject.WebAPI.Tests/      # Tests de l'API
    │   ├── Controllers/
    │   ├── Middlewares/
    │   ├── Integration/
    │   └── MyProject.WebAPI.Tests.csproj
    │
    └── 📦 MyProject.Tests.Common/      # Utilitaires de test
        ├── Fixtures/
        ├── Builders/
        ├── TestData/
        └── MyProject.Tests.Common.csproj
```

## 📋 Fichiers de configuration à la racine

```
MyProject/
├── src/ (structure ci-dessus)
├── tests/
├── docs/
├── scripts/
├── .gitignore
├── .editorconfig
├── README.md
├── MyProject.sln                       # Solution file
├── Directory.Build.props               # Propriétés communes
├── docker-compose.yml                  # Docker configuration
├── Dockerfile
└── appsettings.json                    # Configuration globale
```

## 🎯 Avantages de cette approche par projets

### ✅ **Séparation claire des responsabilités**
- Chaque projet a un rôle spécifique et bien défini
- Facilite la maintenance et l'évolution du code
- Améliore la lisibilité et la compréhension

### ✅ **Gestion fine des dépendances**
- Contrôle précis de ce qui peut référencer quoi
- Évite les dépendances circulaires
- Compilation plus rapide (compilation partielle)

### ✅ **Réutilisabilité**
- Les DTOs peuvent être réutilisés dans différents projets
- Les interfaces peuvent être partagées facilement
- Les projets Shared sont utilisables partout

### ✅ **Tests isolés**
- Chaque projet peut être testé indépendamment
- Mock plus facile des dépendances
- Tests plus rapides et ciblés

### ✅ **Déploiement modulaire**
- Possibilité de déployer des parties spécifiques
- Microservices plus faciles à extraire
- Mise à jour partielle possible

## 🔗 Références détaillées entre projets et justifications

### 📦 **MyProject.Domain** (Cœur - Aucune référence)
```xml
<PackageReference Include="FluentValidation" Version="11.0.0" />
```
**Justification :** Le domaine ne dépend de RIEN ! C'est le principe fondamental de la Clean Architecture.
- ❌ Pas de référence à d'autres projets
- ❌ Pas de dépendance vers Entity Framework, ASP.NET, etc.
- ✅ Seulement des packages purement métier (FluentValidation, MediatR.Contracts)

---

### 📦 **MyProject.Application.DTOs**
```xml
<!-- Références aux projets -->
<ProjectReference Include="..\1-Domain\MyProject.Domain\MyProject.Domain.csproj" />
<ProjectReference Include="..\5-Shared\MyProject.Shared.Enums\MyProject.Shared.Enums.csproj" />

<!-- Packages -->
<PackageReference Include="System.ComponentModel.Annotations" Version="5.0.0" />
```
**Justification :** Les DTOs ont besoin du domaine pour mapper les entités
- ✅ **Domain** : Pour connaître les entités à mapper
- ✅ **Shared.Enums** : Pour utiliser les énumérations communes
- ❌ Pas d'autres dépendances pour rester léger et réutilisable

---

### 📦 **MyProject.Application.Interfaces**
```xml
<!-- Références aux projets -->
<ProjectReference Include="..\1-Domain\MyProject.Domain\MyProject.Domain.csproj" />
<ProjectReference Include="MyProject.Application.DTOs\MyProject.Application.DTOs.csproj" />
<ProjectReference Include="..\5-Shared\MyProject.Shared.Constants\MyProject.Shared.Constants.csproj" />
```
**Justification :** Les interfaces définissent les contrats entre couches
- ✅ **Domain** : Pour utiliser les entités dans les signatures
- ✅ **DTOs** : Pour les paramètres et retours des méthodes
- ✅ **Shared.Constants** : Pour les constantes dans les contrats

---

### 📦 **MyProject.Application.UseCases**
```xml
<!-- Références aux projets -->
<ProjectReference Include="..\1-Domain\MyProject.Domain\MyProject.Domain.csproj" />
<ProjectReference Include="MyProject.Application.DTOs\MyProject.Application.DTOs.csproj" />
<ProjectReference Include="MyProject.Application.Interfaces\MyProject.Application.Interfaces.csproj" />
<ProjectReference Include="..\5-Shared\MyProject.Shared.Extensions\MyProject.Shared.Extensions.csproj" />

<!-- Packages -->
<PackageReference Include="MediatR" Version="12.0.0" />
<PackageReference Include="FluentValidation" Version="11.0.0" />
<PackageReference Include="AutoMapper" Version="12.0.0" />
```
**Justification :** Les Use Cases orchestrent la logique métier
- ✅ **Domain** : Pour utiliser les entités et services du domaine
- ✅ **DTOs** : Pour les données d'entrée/sortie
- ✅ **Interfaces** : Pour appeler les services externes
- ✅ **Shared.Extensions** : Pour les méthodes utilitaires
- ✅ **MediatR** : Pattern CQRS pour découpler les handlers

---

### 📦 **MyProject.Application.Mappings**
```xml
<!-- Références aux projets -->
<ProjectReference Include="..\1-Domain\MyProject.Domain\MyProject.Domain.csproj" />
<ProjectReference Include="MyProject.Application.DTOs\MyProject.Application.DTOs.csproj" />

<!-- Packages -->
<PackageReference Include="AutoMapper" Version="12.0.0" />
<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.0.0" />
```
**Justification :** Les mappings transforment entre entités et DTOs
- ✅ **Domain** : Pour mapper depuis les entités
- ✅ **DTOs** : Pour mapper vers les DTOs
- ✅ **AutoMapper** : Facilite les transformations complexes

---

### 📦 **MyProject.Infrastructure.Data**
```xml
<!-- Références aux projets -->
<ProjectReference Include="..\..\1-Domain\MyProject.Domain\MyProject.Domain.csproj" />
<ProjectReference Include="..\..\2-Application\MyProject.Application.Interfaces\MyProject.Application.Interfaces.csproj" />
<ProjectReference Include="..\..\5-Shared\MyProject.Shared.Constants\MyProject.Shared.Constants.csproj" />

<!-- Packages -->
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="7.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="7.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="7.0.0" />
```
**Justification :** L'infrastructure implémente les contrats du domaine
- ✅ **Domain** : Pour implémenter les interfaces des repositories
- ✅ **Application.Interfaces** : Si certaines interfaces sont définies dans Application
- ✅ **Shared.Constants** : Pour les chaînes de connexion, etc.
- ✅ **Entity Framework** : ORM pour l'accès aux données

---

### 📦 **MyProject.Infrastructure.Services**
```xml
<!-- Références aux projets -->
<ProjectReference Include="..\..\1-Domain\MyProject.Domain\MyProject.Domain.csproj" />
<ProjectReference Include="..\..\2-Application\MyProject.Application.Interfaces\MyProject.Application.Interfaces.csproj" />
<ProjectReference Include="..\..\2-Application\MyProject.Application.DTOs\MyProject.Application.DTOs.csproj" />
<ProjectReference Include="..\..\5-Shared\MyProject.Shared.Helpers\MyProject.Shared.Helpers.csproj" />

<!-- Packages -->
<PackageReference Include="Microsoft.Extensions.Configuration" Version="7.0.0" />
<PackageReference Include="Microsoft.Extensions.Logging" Version="7.0.0" />
<PackageReference Include="System.Security.Cryptography" Version="7.0.0" />
```
**Justification :** Les services implémentent les fonctionnalités techniques
- ✅ **Domain** : Pour les interfaces des services du domaine
- ✅ **Application.Interfaces** : Pour implémenter les contrats
- ✅ **Application.DTOs** : Pour manipuler les données
- ✅ **Shared.Helpers** : Pour les utilitaires de chiffrement, etc.

---

### 📦 **MyProject.Infrastructure.ExternalAPIs**
```xml
<!-- Références aux projets -->
<ProjectReference Include="..\..\2-Application\MyProject.Application.Interfaces\MyProject.Application.Interfaces.csproj" />
<ProjectReference Include="..\..\2-Application\MyProject.Application.DTOs\MyProject.Application.DTOs.csproj" />
<ProjectReference Include="..\..\5-Shared\MyProject.Shared.Constants\MyProject.Shared.Constants.csproj" />

<!-- Packages -->
<PackageReference Include="Stripe.net" Version="42.0.0" />
<PackageReference Include="PayPal.Core.SDK" Version="1.7.1" />
<PackageReference Include="Microsoft.Extensions.Http" Version="7.0.0" />
```
**Justification :** Intégrations avec des APIs tierces
- ✅ **Application.Interfaces** : Pour implémenter les contrats (IPaymentGateway)
- ✅ **Application.DTOs** : Pour échanger des données
- ✅ **Shared.Constants** : Pour les URLs, clés API, etc.
- ❌ **PAS Domain** : Les APIs externes ne doivent pas connaître le domaine directement

---

### 📦 **MyProject.WebAPI**
```xml
<!-- Références aux projets -->
<ProjectReference Include="..\1-Domain\MyProject.Domain\MyProject.Domain.csproj" />
<ProjectReference Include="..\2-Application\MyProject.Application.UseCases\MyProject.Application.UseCases.csproj" />
<ProjectReference Include="..\2-Application\MyProject.Application.DTOs\MyProject.Application.DTOs.csproj" />
<ProjectReference Include="..\2-Application\MyProject.Application.Mappings\MyProject.Application.Mappings.csproj" />
<ProjectReference Include="..\3-Infrastructure\MyProject.Infrastructure.Data\MyProject.Infrastructure.Data.csproj" />
<ProjectReference Include="..\3-Infrastructure\MyProject.Infrastructure.Services\MyProject.Infrastructure.Services.csproj" />
<ProjectReference Include="..\3-Infrastructure\MyProject.Infrastructure.Common\MyProject.Infrastructure.Common.csproj" />

<!-- Packages -->
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="7.0.0" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
<PackageReference Include="MediatR.Extensions.Microsoft.DependencyInjection" Version="11.0.0" />
```
**Justification :** Point d'entrée de l'application - Composition Root
- ✅ **Toutes les couches** : Car c'est le point d'assemblage (DI Container)
- ✅ **Domain** : Pour l'injection de dépendance
- ✅ **Application.UseCases** : Pour envoyer les commandes/queries
- ✅ **Application.DTOs** : Pour les réponses HTTP
- ✅ **Infrastructure** : Pour configurer les implémentations

---

### 📦 **Projets Shared** (Aucune référence métier)
```xml
<!-- Seulement des packages utilitaires -->
<PackageReference Include="System.ComponentModel.Annotations" Version="5.0.0" />
<PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
```
**Justification :** Les projets partagés doivent être indépendants
- ❌ Aucune référence vers les autres projets métier
- ✅ Seulement des packages utilitaires génériques

---

## 🚨 Règles STRICTES à respecter

### ❌ **INTERDICTIONS**
```xml
<!-- ❌ JAMAIS dans Domain -->
<ProjectReference Include="Infrastructure.*" />
<ProjectReference Include="Application.*" />
<PackageReference Include="Microsoft.EntityFrameworkCore" />

<!-- ❌ JAMAIS dans Application -->
<ProjectReference Include="Infrastructure.*" />
<ProjectReference Include="WebAPI.*" />
<PackageReference Include="Microsoft.EntityFrameworkCore" />

<!-- ❌ JAMAIS dans Infrastructure -->
<ProjectReference Include="WebAPI.*" />
<ProjectReference Include="MVC.*" />
```

### ✅ **AUTORISATIONS**
```xml
<!-- ✅ Dans Infrastructure -->
<ProjectReference Include="Domain" />
<ProjectReference Include="Application.Interfaces" />
<ProjectReference Include="Application.DTOs" />

<!-- ✅ Dans Presentation -->
<ProjectReference Include="Domain" />
<ProjectReference Include="Application.*" />
<ProjectReference Include="Infrastructure.*" />
```

## 🎯 **Pourquoi ces règles ?**

### **🔒 Principe d'inversion de dépendance**
- Les couches externes dépendent des couches internes
- Jamais l'inverse !

### **🧪 Testabilité**
- Le Domain peut être testé sans infrastructure
- L'Application peut être testée avec des mocks

### **🔄 Flexibilité**
- Changer de base de données = changer Infrastructure.Data
- Changer d'API = changer seulement WebAPI
- Le cœur métier reste intact

### **📦 Réutilisabilité**
- Les DTOs peuvent être utilisés dans une app mobile
- Le Domain peut être utilisé dans un autre contexte
- Les Use Cases restent les mêmes peu importe l'interface