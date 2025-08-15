using Application.Abstractions.Handlers;
using Application.Abstractions.Services;

namespace Application.UseCases.Users.Delete;

public class DeleteUserCommandHandler(IUsersService usersService) : ICommandHandler<DeleteUserCommand>
{
    public Task HandleAsync(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        return usersService.DeleteAsync(command.Id, cancellationToken);
    }
}
