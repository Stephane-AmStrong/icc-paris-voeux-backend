#nullable enable
using Application.DataTransfertObjects;

namespace Application.Services.Abstractions;

public interface IWishesService
{
    Task<PagedListResponse<WishResponse>> GetPagedListByQueryAsync(WishQueryParameters queryParameters, CancellationToken cancellationToken);
    Task<WishResponse?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<WishResponse> CreateAsync(WishCreateRequest wishRequest, CancellationToken cancellationToken);
    Task UpdateAsync(string id, WishUpdateRequest wishRequest, CancellationToken cancellationToken);
    Task DeleteAsync(string id, CancellationToken cancellationToken);
}
