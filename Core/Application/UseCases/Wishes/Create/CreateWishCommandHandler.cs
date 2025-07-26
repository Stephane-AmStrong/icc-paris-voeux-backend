using Application.Abstractions.Handlers;
using Application.Abstractions.Services;
using Application.UseCases.Wishes.GetByQuery;

namespace Application.UseCases.Wishes.Create;

public class CreateWishCommandHandler(IWishesService wishesService) : ICommandHandler<CreateWishCommand, WishResponse>
{
    public async Task<WishResponse> HandleAsync(CreateWishCommand command, CancellationToken cancellationToken)
    {
        return await wishesService.CreateAsync(command.Payload, cancellationToken);
    }
}
