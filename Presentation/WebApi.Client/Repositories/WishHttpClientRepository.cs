using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using WebApi.Client.DataTransferObjects;
using WebApi.Client.Paging;

namespace WebApi.Client.Repositories;

public class WishHttpClientRepository(HttpClient httpClient, ILogger logger) : BaseHttpClientRepository(httpClient, logger, Endpoint, Entity), IWishHttpClientRepository
{
    private const string Entity = "wish";
    private const string Endpoint = $"api/{Entity}s";

    public Task<WishResponse> CreateAsync(WishCreateRequest createRequest, CancellationToken cancellationToken)
    {
        return BaseCreateAsync<WishCreateRequest, WishResponse>(createRequest, cancellationToken);
    }

    public Task DeleteAsync(string wishId, CancellationToken cancellationToken)
    {
        return BaseDeleteAsync(wishId, cancellationToken);
    }

    public Task<WishResponse> GetByIdAsync(string wishId, CancellationToken cancellationToken)
    {
        return BaseGetByIdAsync<WishResponse>(wishId, cancellationToken);
    }

    public Task<PagedList<WishResponse>> GetPagedListAsync(WishQuery query, CancellationToken cancellationToken)
    {
        return BaseGetPagedListAsync<WishResponse>(query, cancellationToken);
    }

    public Task UpdateAsync(string wishId, WishUpdateRequest wishRequest, CancellationToken cancellationToken)
    {
        return BaseUpdateAsync(wishId, wishRequest, cancellationToken);
    }

    protected override List<KeyValuePair<string, StringValues>> AddSpecificQueryParameters(QueryParameters query)
    {
        var specificParams = new List<KeyValuePair<string, StringValues>>();

        if (query is WishQuery wishQuery)
        {
            if (!string.IsNullOrWhiteSpace(wishQuery.WithEmail))
                specificParams.Add(KeyValuePair.Create(nameof(wishQuery.WithEmail), new StringValues(wishQuery.WithEmail)));

            if (!string.IsNullOrWhiteSpace(wishQuery.WithSpiritually))
                specificParams.Add(KeyValuePair.Create(nameof(wishQuery.WithSpiritually), new StringValues(wishQuery.WithSpiritually)));
        }

        return specificParams;
    }
}
