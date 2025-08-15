#nullable enable
using Application.UseCases.Wishes.GetByQuery;
using Domain.Shared.Common;
using DataTransfertObjects.Requests;
using DataTransfertObjects.Responses;

namespace Application.Abstractions.Services;

public interface IWishesService
{
    Task<PagedList<WishResponse>> GetPagedListByQueryAsync(WishQuery query, CancellationToken cancellationToken);
    Task<WishDetailedResponse?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<WishResponse> CreateAsync(WishCreateRequest wishRequest, CancellationToken cancellationToken);
    Task UpdateAsync(string id, WishUpdateRequest wishRequest, CancellationToken cancellationToken);
    Task DeleteAsync(string id, CancellationToken cancellationToken);
}
