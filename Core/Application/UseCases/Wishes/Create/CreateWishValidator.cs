using Application.Common;
using Domain.Repositories.Abstractions;
using FluentValidation;

namespace Application.UseCases.Wishes.Create;

public class CreateWishValidator : AbstractValidator<CreateWishCommand>
{
    public CreateWishValidator(IWishesRepository wishesRepository)
    {
        RuleFor(command => command.Payload.Email)
            .NotEmpty()
            .WithMessage(Validation.Messages.FieldRequired)
            .MustAsync(async (email, cancellationToken) =>
            {
                var wish = (await wishesRepository.FindByConditionAsync(wish => wish.Email == email, cancellationToken)).FirstOrDefault();
                return wish == null;
            })
            .WithMessage(string.Format(Validation.Messages.EntityAlreadyExists, Validation.Entities.Wish));

        RuleFor(command => command.Payload)
            .Must(HaveAtLeastOnePropertySet)
            .WithMessage(string.Format(Validation.Messages.AtLeastOnePropertyRequired, Validation.Entities.Wish, "Spiritually, FamiliallyRelationally, FinanciallyMaterially, ProfessionallyAcademically, Other"));
    }

    private static bool HaveAtLeastOnePropertySet(WishCreateRequest wish) => !string.IsNullOrWhiteSpace(wish.Spiritually) || !string.IsNullOrWhiteSpace(wish.FamiliallyRelationally) || !string.IsNullOrWhiteSpace(wish.FinanciallyMaterially) || !string.IsNullOrWhiteSpace(wish.ProfessionallyAcademically) || !string.IsNullOrWhiteSpace(wish.Other);
}