using Application.Common;
using Domain.Abstractions.Repositories;
using FluentValidation;

namespace Application.UseCases.Wishes.Create;

public class CreateWishValidator : AbstractValidator<CreateWishCommand>
{
    public CreateWishValidator(IUsersRepository usersRepository)
    {
        RuleFor(command => command.Payload.Title)
            .NotNull()
            .WithMessage(Validation.Messages.FieldRequired)
            .OverridePropertyName(nameof(CreateWishCommand.Payload.Title));

        RuleFor(command => command.Payload.Type)
            .NotNull()
            .WithMessage(Validation.Messages.FieldRequired)
            .OverridePropertyName(nameof(CreateWishCommand.Payload.Type));

        RuleFor(command => command.Payload.UserId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(Validation.Messages.FieldRequired)
            .OverridePropertyName(nameof(CreateWishCommand.Payload.UserId))
            .MustAsync(async (userId, cancellationToken) =>
            {
                var user = await usersRepository.GetByIdAsync(userId, cancellationToken);
                return user is not null;
            })
            .WithMessage(string.Format(Validation.Messages.EntityNotFound, Validation.Entities.User));
    }
}
