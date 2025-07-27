using Application.Abstractions.Handlers;
using Application.Common;
using FluentValidation;
using FluentValidation.Results;

namespace Application.Abstractions.Behaviors;

public static class ValidationDecorator
{
    public sealed class CommandHandler<TCommand, TResponse>(ICommandHandler<TCommand, TResponse> innerHandler, IValidator<TCommand> validator) : ICommandHandler<TCommand, TResponse> where TCommand : ICommand<TResponse>
    {
        public async Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            ValidationFailure[] validationFailures = await ValidateAsync(command, validator);

            if (validationFailures.Length == 0)
            {
                return await innerHandler.HandleAsync(command, cancellationToken);
            }

            throw new ValidationError(validationFailures);
        }
    }

    public sealed class CommandBaseHandler<TCommand>(ICommandHandler<TCommand> innerHandler, IValidator<TCommand> validator): ICommandHandler<TCommand> where TCommand : ICommand
    {
        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            ValidationFailure[] validationFailures = await ValidateAsync(command, validator);

            if (validationFailures.Length == 0)
            {
                await innerHandler.HandleAsync(command, cancellationToken);
            }

            throw new ValidationError(validationFailures);
        }
    }

    private static async Task<ValidationFailure[]> ValidateAsync<TCommand>(TCommand command, IValidator<TCommand> validator)
    {
        if (validator == null)
        {
            return [];
        }

        var context = new ValidationContext<TCommand>(command);

        ValidationResult validationResult = await validator.ValidateAsync(context);

        ValidationFailure[] validationFailures = validationResult.Errors.ToArray();

        return validationFailures;
    }
}
