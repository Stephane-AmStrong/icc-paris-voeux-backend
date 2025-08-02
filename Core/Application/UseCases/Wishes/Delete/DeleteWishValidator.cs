using Application.Common;
using Domain.Repositories.Abstractions;
using FluentValidation;

namespace Application.UseCases.Wishes.Delete;

public class DeleteWishValidator : AbstractValidator<DeleteWishCommand>
{
    public DeleteWishValidator(IWishesRepository wishesRepository)
    {
        RuleFor(command => command.Id)
            .NotEmpty()
            .WithMessage(Validation.Messages.FieldRequired)
            .MustAsync(async (id, cancellationToken) =>
            {
                var wish = await wishesRepository.GetByIdAsync(id, cancellationToken);
                return wish != null;
            })
            .WithMessage(string.Format(Validation.Messages.EntityNotFound, Validation.Entities.Wish));
    }
}