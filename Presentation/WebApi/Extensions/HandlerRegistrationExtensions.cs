using Application.Abstractions.Decorators;
using Application.Abstractions.Handlers;
using FluentValidation;

namespace WebApi.Extensions;

public static class HandlerRegistrationExtensions
{
    public static void AddCommandWithValidation<TCommand, THandler, TValidator, TResponse>(this IServiceCollection services)
    where TCommand : ICommand<TResponse>
    where THandler : class, ICommandHandler<TCommand, TResponse>
    where TValidator : class, IValidator<TCommand>
    {
        services.AddScoped<THandler>();
        services.AddScoped<ICommandHandler<TCommand, TResponse>>(provider =>
        {
            var handler = provider.GetRequiredService<THandler>();
            var validator = provider.GetRequiredService<TValidator>();
            return new ValidationDecorator.CommandHandler<TCommand, TResponse>(handler, validator);
        });
    }

    public static void AddCommandWithValidation<TCommand, THandler, TValidator>(this IServiceCollection services)
    where TCommand : ICommand
    where THandler : class, ICommandHandler<TCommand>
    where TValidator : class, IValidator<TCommand>
    {
        services.AddScoped<THandler>();
        services.AddScoped<ICommandHandler<TCommand>>(provider =>
        {
            var handler = provider.GetRequiredService<THandler>();
            var validator = provider.GetRequiredService<TValidator>();
            return new ValidationDecorator.CommandBaseHandler<TCommand>(handler, validator);
        });
    }

    public static void AddQueryWithValidation<TQuery, THandler, TValidator, TResponse>(this IServiceCollection services)
    where TQuery : IQuery<TResponse>
    where THandler : class, IQueryHandler<TQuery, TResponse>
    where TValidator : class, IValidator<TQuery>
    {
        services.AddScoped<THandler>();
        services.AddScoped<IQueryHandler<TQuery, TResponse>>(provider =>
        {
            var handler = provider.GetRequiredService<THandler>();
            var validator = provider.GetRequiredService<TValidator>();
            return new ValidationDecorator.QueryHandler<TQuery, TResponse>(handler, validator);
        });
    }

}