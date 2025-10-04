#nullable enable
using Application.UseCases.Servers.GetByQuery;
using DataTransfertObjects.Requests;
using DataTransfertObjects.Responses;
using Domain.Shared.Common;

namespace Application.Abstractions.Services;

public interface IServersService
{
    Task<PagedList<ServerResponse>> GetPagedListByQueryAsync(ServerQuery query, CancellationToken cancellationToken);
    Task<ServerDetailedResponse?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<ServerResponse> CreateAsync(ServerCreateRequest serverRequest, CancellationToken cancellationToken);
    Task UpdateAsync(string id, ServerUpdateRequest serverRequest, CancellationToken cancellationToken);
    Task DeleteAsync(string id, CancellationToken cancellationToken);
}
