using Application.Abstractions.Handlers;
using Application.Abstractions.Services;

namespace Application.UseCases.Wishes.Delete;

public class DeleteWishCommandHandler(IWishesService wishesService) : ICommandHandler<DeleteWishCommand>
{
    public Task HandleAsync(DeleteWishCommand command, CancellationToken cancellationToken)
    {
        return wishesService.DeleteAsync(command.Id, cancellationToken);
    }
}
