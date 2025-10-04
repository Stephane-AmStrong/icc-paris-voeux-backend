using Application.Abstractions.Handlers;
using Application.Abstractions.Services;
using DataTransfertObjects.Responses;

namespace Application.UseCases.ServerStatuses.GetById;

public class GetServerStatusByIdQueryHandler(IServerStatusesService serverStatusesService) : IQueryHandler<GetServerStatusByIdQuery, ServerStatusDetailedResponse>
{
    public Task<ServerStatusDetailedResponse> HandleAsync(GetServerStatusByIdQuery query, CancellationToken cancellationToken)
    {
        return serverStatusesService.GetByIdAsync(query.Id, cancellationToken);
    }
}
