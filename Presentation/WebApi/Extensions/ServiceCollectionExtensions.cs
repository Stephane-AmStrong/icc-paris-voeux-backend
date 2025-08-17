using System.Text.Json.Serialization;
using Application.Abstractions.Handlers;
using Application.Abstractions.Services;
using Application.UseCases.Wishes.Create;
using Application.UseCases.Wishes.Delete;
using Application.UseCases.Wishes.GetById;
using Application.UseCases.Wishes.GetByQuery;
using Application.UseCases.Wishes.Update;
using Application.UseCases.Users.Create;
using Application.UseCases.Users.Delete;
using Application.UseCases.Users.GetById;
using Application.UseCases.Users.GetByQuery;
using Application.UseCases.Users.Update;
using Domain.Abstractions.Repositories;
using Domain.Shared.Common;
using FluentValidation;
using DataTransfertObjects.Responses;
using Domain.Abstractions.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Persistence.Events;
using Persistence.Repository;
using Services;
using WebApi.Middleware;
using Domain.Events;

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
                builder.WithOrigins(allowedOrigins)
                //builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
            });

        });
    }

    public static void ConfigureMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MongoDB");
        var databaseName = configuration.GetConnectionString("DatabaseName");

        services.AddSingleton<IMongoClient>(serviceProvider => new MongoClient(connectionString));

        services.AddScoped(serviceProvider =>
        {
            var client = serviceProvider.GetRequiredService<IMongoClient>();
            return client.GetDatabase(databaseName);
        });

    }

    public static void ConfigureValidation(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateUserCommand>, CreateUserValidator>();
        services.AddScoped<IValidator<UpdateUserCommand>, UpdateUserValidator>();
        services.AddScoped<IValidator<DeleteUserCommand>, DeleteUserValidator>();

        services.AddScoped<IValidator<CreateWishCommand>, CreateWishValidator>();
        services.AddScoped<IValidator<UpdateWishCommand>, UpdateWishValidator>();
        services.AddScoped<IValidator<DeleteWishCommand>, DeleteWishValidator>();
    }

    public static void ConfigureHandlers(this IServiceCollection services)
    {
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
    }

    public static void ConfigureDomainEventHandlers(this IServiceCollection services)
    {
        services.AddScoped<IEventHandler<UserCreatedEvent>, UserCreatedEventHandler>();
        services.AddScoped<IEventHandler<UserDeletedEvent>, UserDeletedEventHandler>();
        services.AddScoped<IEventHandler<UserUpdatedEvent>, UserUpdatedEventHandler>();
    }

    public static void ConfigureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IWishesRepository, WishesRepository>();
        services.AddScoped<IPulsesRepository, PulsesRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();
    }

    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IEventsDispatcher, EventsDispatcher>();
        services.AddScoped<IWishesService, WishesService>();
        services.AddScoped<IUsersService, UsersService>();
    }


    public static void ConfigureSwagger(this IServiceCollection services)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "ICCParis Web API", Version = "v1" }));
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

    public static RouteHandlerBuilder WithRequestValidation<TRequest>(this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter<Filters.ValidationFilter<TRequest>>().ProducesValidationProblem();
    }
}
