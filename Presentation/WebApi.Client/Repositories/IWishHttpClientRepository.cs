using WebApi.Client.DataTransferObjects;
using WebApi.Client.Paging;

namespace WebApi.Client.Repositories;

public interface IWishHttpClientRepository
{
    Task<WishResponse> CreateAsync(WishCreateRequest createRequest, CancellationToken cancellationToken);
    Task DeleteAsync(string alertId, CancellationToken cancellationToken);
    Task<WishResponse> GetByIdAsync(string alertId, CancellationToken cancellationToken);
    Task<PagedList<WishResponse>> GetPagedListAsync(WishQuery query, CancellationToken cancellationToken);
    Task UpdateAsync(string alertId, WishUpdateRequest wishRequest, CancellationToken cancellationToken);
}
