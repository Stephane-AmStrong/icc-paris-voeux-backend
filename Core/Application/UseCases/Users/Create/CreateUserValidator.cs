using Application.Common;
using Domain.Abstractions.Repositories;
using FluentValidation;

namespace Application.UseCases.Users.Create;

public class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator(IUsersRepository usersRepository)
    {
        RuleFor(command => command.Payload.LastName)
            .NotEmpty()
            .WithMessage(Validation.Messages.FieldRequired)
            .OverridePropertyName(nameof(CreateUserCommand.Payload.LastName));

        RuleFor(command => command.Payload.FirstName)
            .NotEmpty()
            .WithMessage(Validation.Messages.FieldRequired)
            .OverridePropertyName(nameof(CreateUserCommand.Payload.FirstName));
    }
}
