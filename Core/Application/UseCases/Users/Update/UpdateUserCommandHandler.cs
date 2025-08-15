using Application.Abstractions.Handlers;
using Application.Abstractions.Services;

namespace Application.UseCases.Users.Update;

public class UpdateUserCommandHandler(IUsersService usersService) : ICommandHandler<UpdateUserCommand>
{
    public Task HandleAsync(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        return usersService.UpdateAsync(command.Id, command.Payload, cancellationToken);
    }
}
