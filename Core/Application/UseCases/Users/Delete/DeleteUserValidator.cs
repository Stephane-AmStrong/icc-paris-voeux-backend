using Application.Common;
using Domain.Abstractions.Repositories;
using FluentValidation;

namespace Application.UseCases.Users.Delete;

public class DeleteUserValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserValidator(IUsersRepository usersRepository)
    {
        RuleFor(command => command.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(string.Format(Validation.Messages.FieldRequired))
            .MustAsync(async (userId, cancellationToken) =>
            {
                var user = await usersRepository.GetByIdAsync(userId, cancellationToken);
                return user != null;
            }).WithMessage(string.Format(Validation.Messages.EntityNotFound, Validation.Entities.User));

    }
}
