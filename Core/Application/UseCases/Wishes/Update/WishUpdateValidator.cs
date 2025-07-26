using Application.Common;
using Domain.Repositories.Abstractions;
using FluentValidation;

namespace Application.UseCases.Wishes.Update;

public class WishUpdateValidator : AbstractValidator<UpdateWishCommand>
{
    public WishUpdateValidator(IWishesRepository wishesRepository)
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

        RuleFor(command => command.Payload.Email)
            .NotEmpty()
            .WithMessage(Validation.Messages.FieldRequired);

        RuleFor(command => command.Payload)
            .Must(HaveAtLeastOnePropertySet)
            .WithMessage(string.Format(Validation.Messages.AtLeastOnePropertyRequired, Validation.Entities.Wish, "Spiritually, FamiliallyRelationally, FinanciallyMaterially, ProfessionallyAcademically, Other"));
    }

    private static bool HaveAtLeastOnePropertySet(WishUpdateRequest wish) => !string.IsNullOrWhiteSpace(wish.Spiritually) || !string.IsNullOrWhiteSpace(wish.FamiliallyRelationally) || !string.IsNullOrWhiteSpace(wish.FinanciallyMaterially) || !string.IsNullOrWhiteSpace(wish.ProfessionallyAcademically) || !string.IsNullOrWhiteSpace(wish.Other);

}
