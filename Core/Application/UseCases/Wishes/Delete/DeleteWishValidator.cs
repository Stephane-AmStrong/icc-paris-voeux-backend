using Application.Common;
using Domain.Abstractions.Repositories;
using FluentValidation;

namespace Application.UseCases.Wishes.Delete;

public class DeleteWishValidator : AbstractValidator<DeleteWishCommand>
{
    public DeleteWishValidator(IWishesRepository wishesRepository)
    {
        RuleFor(command => command.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(string.Format(Validation.Messages.FieldRequired))
            .MustAsync(async (wishId, cancellationToken) =>
            {
                var wish = await wishesRepository.GetByIdAsync(wishId, cancellationToken);
                return wish != null;
            }).WithMessage(string.Format(Validation.Messages.EntityNotFound, Validation.Entities.Wish));

    }
}
