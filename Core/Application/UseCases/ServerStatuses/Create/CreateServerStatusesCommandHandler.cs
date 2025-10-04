using Application.Abstractions.Handlers;
using Application.Abstractions.Services;
using DataTransfertObjects.Responses;

namespace Application.UseCases.ServerStatuses.Create;

public class CreateServerStatusCommandHandler(IServerStatusesService serverStatusesService)
    : ICommandHandler<CreateServerStatusCommand, ServerStatusResponse>
{
    public Task<ServerStatusResponse> HandleAsync(CreateServerStatusCommand command, CancellationToken cancellationToken)
    {
        return serverStatusesService.CreateAsync(command.Payload, cancellationToken);
    }
}
