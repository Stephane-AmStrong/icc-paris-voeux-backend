using Application.Abstractions.Handlers;
using Application.Abstractions.Services;

namespace Application.UseCases.Wishes.Delete;

public class DeleteWishCommand : ICommand
{
    public string Id { get; set; }
}

public class DeleteWishCommandHandler(IWishesService wishesService) : ICommandHandler<DeleteWishCommand>
{
    public async Task HandleAsync(DeleteWishCommand command, CancellationToken cancellationToken)
    {
        await wishesService.DeleteAsync(command.Id, cancellationToken);
    }
}
