using DataTransfertObjects.Enumerations;
using FluentValidation;

namespace Application.UseCases.Wishes.GetByQuery;

public class GetWishQueryValidator : AbstractValidator<GetWishQuery>
{
    public GetWishQueryValidator()
    {
        RuleFor(command => command.Parameters.OfType)
            .Must(ofType => ofType == null || Enum.IsDefined(typeof(WishType), ofType))
            .WithMessage("OfType must be a valid WishType value when specified")
            .OverridePropertyName(nameof(GetWishQuery.Parameters.OfType));
    }
}
