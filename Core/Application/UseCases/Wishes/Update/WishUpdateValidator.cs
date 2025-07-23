using Domain.Repositories.Abstractions;
using FluentValidation;

namespace Application.UseCases.Wishes.Update;

public class WishUpdateValidator : AbstractValidator<WishUpdateRequest>
{
    public WishUpdateValidator(IWishesRepository wishesRepository)
    {
        RuleFor(wish => wish)
            .Must(HaveAtLeastOnePropertySet)
            .WithMessage("At least one of the wish properties must be set: Spiritually, FamiliallyRelationally, FinanciallyMaterially, ProfessionallyAcademically, Other");
            
        /*
        RuleFor(wish => wish.Email)
            .NotEmpty()
            .MustAsync(async (wishId, cancellationToken) =>
            {
                var wish = await wishesRepository.GetByIdAsync(wishId, cancellationToken);
                return wish != null;
            })
            .WithMessage("Wish  with email '{PropertyValue}' doesn't already exist");
        */
    }

    private static bool HaveAtLeastOnePropertySet(WishUpdateRequest request)
    {
        return !string.IsNullOrWhiteSpace(request.Spiritually) || !string.IsNullOrWhiteSpace(request.FamiliallyRelationally) || !string.IsNullOrWhiteSpace(request.FinanciallyMaterially) || !string.IsNullOrWhiteSpace(request.ProfessionallyAcademically) || !string.IsNullOrWhiteSpace(request.Other);
    }
}
