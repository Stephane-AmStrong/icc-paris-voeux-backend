#nullable enable
using Application.UseCases.ServerStatuses.GetByQuery;
using DataTransfertObjects.Requests;
using DataTransfertObjects.Responses;
using Domain.Shared.Common;

namespace Application.Abstractions.Services;

public interface IServerStatusesService
{
    Task<PagedList<ServerStatusResponse>> GetPagedListByQueryAsync(ServerStatusQuery query, CancellationToken cancellationToken);
    Task<ServerStatusDetailedResponse?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<ServerStatusResponse> CreateAsync(ServerStatusCreateRequest serverStatusRequest, CancellationToken cancellationToken);
}
