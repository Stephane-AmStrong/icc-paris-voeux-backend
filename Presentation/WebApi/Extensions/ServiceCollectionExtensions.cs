using System.Text.Json.Serialization;
using Application.Abstractions.Handlers;
using Application.Abstractions.Services;
using Application.UseCases.Alerts.CreateOrIncrement;
using Application.UseCases.Alerts.Delete;
using Application.UseCases.Alerts.GetById;
using Application.UseCases.Alerts.GetByQuery;
using Application.UseCases.Alerts.Update;
using Application.UseCases.Servers.Create;
using Application.UseCases.Servers.Delete;
using Application.UseCases.Servers.GetById;
using Application.UseCases.Servers.GetByQuery;
using Application.UseCases.Servers.Update;
using Application.UseCases.ServerStatuses.Create;
using Application.UseCases.ServerStatuses.GetById;
using Application.UseCases.ServerStatuses.GetByQuery;
using Application.UseCases.Users.Create;
using Application.UseCases.Users.Delete;
using Application.UseCases.Users.GetById;
using Application.UseCases.Users.GetByQuery;
using Application.UseCases.Users.Update;
using Application.UseCases.Wishes.Create;
using Application.UseCases.Wishes.Delete;
using Application.UseCases.Wishes.GetById;
using Application.UseCases.Wishes.GetByQuery;
using Application.UseCases.Wishes.Update;
using DataTransfertObjects.Responses;
using Domain.Abstractions.Events;
using Domain.Abstractions.Repositories;
using Domain.Events;
using Domain.Shared.Common;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using MongoDB.Driver;
using Persistence.Events;
using Persistence.Repository;
using Services;
using WebApi.Middleware;

namespace WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigureCors(this IServiceCollection services, IConfiguration configuration)
    {
        string[] allowedOrigins = configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>() ?? [];

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
            {
                //builder.WithOrigins(allowedOrigins)
                builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
            });

        });
    }

    public static void ConfigureMongoDB(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MongoDB");
        var databaseName = configuration.GetConnectionString("DatabaseName");

        services.AddSingleton<IMongoClient>(serviceProvider =>
        {
            return new MongoClient(connectionString);
        });

        services.AddScoped(serviceProvider =>
        {
            var client = serviceProvider.GetRequiredService<IMongoClient>();
            return client.GetDatabase(databaseName);
        });
    }

    public static void ConfigureFlatConfigurationSync(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IFlatConfigurationService>(serviceProvider =>
        {
            string mcsConfigFlatPath = configuration["SharedFilePaths:McsConfigFlat"]!;
            var logger = serviceProvider.GetRequiredService<ILogger<FlatConfigurationService>>();
            var jsonReader = serviceProvider.GetRequiredService<IJsonFileReader>();

            return new FlatConfigurationService(logger, jsonReader, mcsConfigFlatPath);
        });
    }

    public static void ConfigureValidation(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateOrIncrementAlertCommand>, CreateOrIncrementAlertValidator>();
        services.AddScoped<IValidator<UpdateAlertCommand>, UpdateAlertValidator>();
        services.AddScoped<IValidator<DeleteAlertCommand>, DeleteAlertValidator>();

        services.AddScoped<IValidator<CreateUserCommand>, CreateUserValidator>();
        services.AddScoped<IValidator<UpdateUserCommand>, UpdateUserValidator>();
        services.AddScoped<IValidator<DeleteUserCommand>, DeleteUserValidator>();

        services.AddScoped<IValidator<CreateServerCommand>, CreateServerValidator>();
        services.AddScoped<IValidator<UpdateServerCommand>, UpdateServerValidator>();
        services.AddScoped<IValidator<DeleteServerCommand>, DeleteServerValidator>();

        services.AddScoped<IValidator<CreateServerStatusCommand>, CreateServerStatusValidator>();
    }

    public static void ConfigureHandlers(this IServiceCollection services)
    {
        //Alerts
        services.AddScoped<IQueryHandler<GetAlertByIdQuery, AlertDetailedResponse>, GetAlertByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetAlertQuery, PagedList<AlertResponse>>, GetAlertQueryHandler>();

        services.AddCommandWithValidation<CreateOrIncrementAlertCommand, CreateOrIncrementAlertCommandHandler, IValidator<CreateOrIncrementAlertCommand>, AlertResponse>();
        services.AddCommandWithValidation<UpdateAlertCommand, UpdateAlertCommandHandler, IValidator<UpdateAlertCommand>>();
        services.AddCommandWithValidation<DeleteAlertCommand, DeleteAlertCommandHandler, IValidator<DeleteAlertCommand>>();

        //Users
        services.AddScoped<IQueryHandler<GetUserByIdQuery, UserDetailedResponse>, GetUserByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetUserQuery, PagedList<UserResponse>>, GetUserQueryHandler>();

        services.AddCommandWithValidation<CreateUserCommand, CreateUserCommandHandler, IValidator<CreateUserCommand>, UserResponse>();
        services.AddCommandWithValidation<UpdateUserCommand, UpdateUserCommandHandler, IValidator<UpdateUserCommand>>();
        services.AddCommandWithValidation<DeleteUserCommand, DeleteUserCommandHandler, IValidator<DeleteUserCommand>>();
        
        //Wishes
        services.AddScoped<IQueryHandler<GetWishByIdQuery, WishDetailedResponse>, GetWishByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetWishQuery, PagedList<WishResponse>>, GetWishQueryHandler>();

        services.AddCommandWithValidation<CreateWishCommand, CreateWishCommandHandler, IValidator<CreateWishCommand>, WishResponse>();
        services.AddCommandWithValidation<UpdateWishCommand, UpdateWishCommandHandler, IValidator<UpdateWishCommand>>();
        services.AddCommandWithValidation<DeleteWishCommand, DeleteWishCommandHandler, IValidator<DeleteWishCommand>>();

        //ServerStatuses
        services.AddScoped<IQueryHandler<GetServerStatusByIdQuery, ServerStatusDetailedResponse>, GetServerStatusByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetServerStatusQuery, PagedList<ServerStatusResponse>>, GetServerStatusQueryHandler>();

        services.AddCommandWithValidation<CreateServerStatusCommand, CreateServerStatusCommandHandler, IValidator<CreateServerStatusCommand>, ServerStatusResponse>();

        //Servers
        services.AddScoped<IQueryHandler<GetServerByIdQuery, ServerDetailedResponse>, GetServerByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetServerQuery, PagedList<ServerResponse>>, GetServerQueryHandler>();

        services.AddCommandWithValidation<CreateServerCommand, CreateServerCommandHandler, IValidator<CreateServerCommand>, ServerResponse>();
        services.AddCommandWithValidation<UpdateServerCommand, UpdateServerCommandHandler, IValidator<UpdateServerCommand>>();
        services.AddCommandWithValidation<DeleteServerCommand, DeleteServerCommandHandler, IValidator<DeleteServerCommand>>();
    }

    public static void ConfigureDomainEventHandlers(this IServiceCollection services)
    {
        services.AddScoped<IEventHandler<AlertCreatedEvent>, AlertCreatedEventHandler>();
        services.AddScoped<IEventHandler<AlertUpdatedEvent>, AlertUpdatedEventHandler>();
        services.AddScoped<IEventHandler<AlertDeletedEvent>, AlertDeletedEventHandler>();

        services.AddScoped<IEventHandler<UserCreatedEvent>, UserCreatedEventHandler>();
        
        services.AddScoped<IEventHandler<ServerCreatedEvent>, ServerCreatedEventHandler>();
        services.AddScoped<IEventHandler<ServerUpdatedEvent>, ServerUpdatedEventHandler>();
        services.AddScoped<IEventHandler<ServerDeletedEvent>, ServerDeletedEventHandler>();

        services.AddScoped<IEventHandler<ServerStatusCreatedEvent>, ServerStatusCreatedEventHandler>();
    }

    public static void ConfigureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAlertsRepository, AlertsRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IWishesRepository, WishesRepository>();
        services.AddScoped<IServerStatusesRepository, ServerStatusesRepository>();
        services.AddScoped<IServersRepository, ServersRepository>();
    }

    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IAlertsService, AlertsService>();
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<IWishesService, WishesService>();
        services.AddScoped<IEventsDispatcher, EventsDispatcher>();
        services.AddScoped<IJsonFileReader, JsonFileReader>();
        services.AddScoped<IServerEnvironmentSyncService, ServerConfigurationSyncService>();
        services.AddScoped<IServersService, ServersService>();
        services.AddScoped<IServerStatusesService, ServerStatusesService>();

        //HostedService
        services.AddHostedService<IccBootstrapService>();
    }

    public static void AddOpenApiServices(this IServiceCollection services)
    {
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi();
    }

    public static void UseOpenApiWithSwagger(this WebApplication app)
    {
        app.MapOpenApi();

        // Configure OpenAPI mapping and Swagger UI
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "Icc Web API");
            options.RoutePrefix = string.Empty;
        });
    }

    public static void ConfigureJsonOptions(this IServiceCollection services)
    {
        services.Configure<JsonOptions>(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
    }

    public static void ConfigureGlobalExceptionHandling(this IServiceCollection services)
    {
        services.AddScoped<EndpointLoggingMiddleware>();
        services.AddScoped<ExceptionHandlingMiddleware>();
    }

    public static IServiceCollection AddKestrelConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KestrelServerOptions>(configuration.GetSection("Kestrel"));
        return services;
    }

    public static RouteHandlerBuilder WithRequestValidation<TRequest>(this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter<Filters.ValidationFilter<TRequest>>().ProducesValidationProblem();
    }
}
