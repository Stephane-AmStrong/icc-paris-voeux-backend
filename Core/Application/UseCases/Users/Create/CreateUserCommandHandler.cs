using Application.Abstractions.Handlers;
using Application.Abstractions.Services;
using DataTransfertObjects.Responses;

namespace Application.UseCases.Users.Create;

public class CreateUserCommandHandler(IUsersService usersService)
    : ICommandHandler<CreateUserCommand, UserResponse>
{
    public Task<UserResponse> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken)
    {
        return usersService.CreateAsync(command.Payload, cancellationToken);
    }
}