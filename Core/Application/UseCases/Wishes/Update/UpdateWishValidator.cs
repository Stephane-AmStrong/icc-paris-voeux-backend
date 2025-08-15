using Application.Common;
using Domain.Abstractions.Repositories;
using FluentValidation;

namespace Application.UseCases.Wishes.Update;

public class UpdateWishValidator : AbstractValidator<UpdateWishCommand>
{
    public UpdateWishValidator(IWishesRepository wishesRepository, IUsersRepository usersRepository)
    {
        RuleFor(command => command.Payload.Title)
            .NotNull()
            .WithMessage(Validation.Messages.FieldRequired)
            .OverridePropertyName(nameof(UpdateWishCommand.Payload.Title));

        RuleFor(command => command.Payload.Type)
            .NotNull()
            .WithMessage(Validation.Messages.FieldRequired)
            .OverridePropertyName(nameof(UpdateWishCommand.Payload.Type));

        RuleFor(command => command.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(string.Format(Validation.Messages.FieldRequired))
            .MustAsync(async (wishId, cancellationToken) =>
            {
                var wish = await wishesRepository.GetByIdAsync(wishId, cancellationToken);
                return wish is not null;
            }).WithMessage(string.Format(Validation.Messages.EntityNotFound, Validation.Entities.Wish));

        RuleFor(command => command.Payload.UserId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(Validation.Messages.FieldRequired)
            .OverridePropertyName(nameof(UpdateWishCommand.Payload.UserId))
            .MustAsync(async (userId, cancellationToken) =>
            {
                var user = await usersRepository.GetByIdAsync(userId, cancellationToken);
                return user is not null;
            })
            .WithMessage(string.Format(Validation.Messages.EntityNotFound, Validation.Entities.User));
    }
}
