using System.Text.Json.Serialization;
using Application.Abstractions.Handlers;
using Application.Abstractions.Services;
using Application.UseCases.Wishes.Create;
using Application.UseCases.Wishes.Delete;
using Application.UseCases.Wishes.Update;
using Domain.Entities;
using Domain.Repositories.Abstractions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
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
                builder.WithOrigins(allowedOrigins)
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
            var wish = serviceProvider.GetRequiredService<IMongoClient>();
            return wish.GetDatabase(databaseName);
        });
        
        BsonClassMap.RegisterClassMap<BaseEntity>(cm =>
        {
            cm.AutoMap();
            cm.MapIdMember(c => c.Id)
                .SetIdGenerator(StringObjectIdGenerator.Instance);
        });
    }

    public static void ConfigureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IWishesRepository, WishesRepository>();
    }

    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IWishesService, WishesService>();
    }

    public static void ConfigureHandlers(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<DeleteWishCommand>, DeleteWishCommandHandler>();
    }


    public static void ConfigureSwagger(this IServiceCollection services)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
                        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Icc paris centre Web API", Version = "v1" }));
    }

    public static void ConfigureJsonOptions(this IServiceCollection services)
    {
        services.Configure<JsonOptions>(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
    }

    public static void ConfigureValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<WishCreateValidator>();
        services.AddValidatorsFromAssemblyContaining<WishUpdateValidator>();
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
