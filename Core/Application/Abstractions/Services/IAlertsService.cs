#nullable enable
using Application.UseCases.Alerts.GetByQuery;
using DataTransfertObjects.Requests;
using DataTransfertObjects.Responses;
using Domain.Shared.Common;

namespace Application.Abstractions.Services;

public interface IAlertsService
{
    Task<PagedList<AlertResponse>> GetPagedListByQueryAsync(AlertQuery query, CancellationToken cancellationToken);
    Task<AlertDetailedResponse?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<AlertResponse> CreateOrIncrementAsync(AlertCreateOrIncrementRequest alertRequest, CancellationToken cancellationToken);
    Task UpdateAsync(string id, AlertUpdateRequest alertRequest, CancellationToken cancellationToken);
    Task DeleteAsync(string id, CancellationToken cancellationToken);
}
