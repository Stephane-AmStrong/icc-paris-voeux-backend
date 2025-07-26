using Application.Abstractions.Handlers;
using Application.Abstractions.Services;

namespace Application.UseCases.Wishes.Update;

public class UpdateWishCommandHandler(IWishesService wishesService) : ICommandHandler<UpdateWishCommand>
{
    public async Task HandleAsync(UpdateWishCommand command, CancellationToken cancellationToken)
    {
        await wishesService.UpdateAsync(command.Id, command.Payload, cancellationToken);
    }
}
