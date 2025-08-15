using Application.Common;
using Domain.Abstractions.Repositories;
using FluentValidation;

namespace Application.UseCases.Users.Update;

public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserValidator(IUsersRepository usersRepository)
    {
        RuleFor(user => user.Payload.FirstName)
            .NotEmpty()
            .OverridePropertyName(nameof(UpdateUserCommand.Payload.FirstName))
            .WithMessage(Validation.Messages.FieldRequired);

        RuleFor(user => user.Payload.LastName)
            .NotEmpty()
            .OverridePropertyName(nameof(UpdateUserCommand.Payload.LastName))
            .WithMessage(Validation.Messages.FieldRequired);

        RuleFor(command => command.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(Validation.Messages.FieldRequired)
            .OverridePropertyName(nameof(UpdateUserCommand.Id))
            .MustAsync(async (request, id, cancellationToken) =>
            {
                var existingUser = await usersRepository.GetByIdAsync(id, cancellationToken);
                return existingUser is not null;

            }).WithMessage(string.Format(Validation.Messages.FieldAlreadyInUse, nameof(UpdateUserCommand.Id), Validation.Entities.User));
    }
}
