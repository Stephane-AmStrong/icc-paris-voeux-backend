using Application.Abstractions.Handlers;
using Application.Abstractions.Services;
using DataTransfertObjects.Responses;

namespace Application.UseCases.Wishes.Create;

public class CreateWishCommandHandler(IWishesService wishesService)
    : ICommandHandler<CreateWishCommand, WishResponse>
{
    public Task<WishResponse> HandleAsync(CreateWishCommand command, CancellationToken cancellationToken)
    {
        return wishesService.CreateAsync(command.Payload, cancellationToken);
    }
}
