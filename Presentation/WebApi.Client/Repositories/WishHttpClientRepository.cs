using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using WebApi.Client.DataTransferObjects;
using WebApi.Client.Paging;

namespace WebApi.Client.Repositories;

public class WishHttpClientRepository(HttpClient httpClient, ILogger logger) : BaseHttpClientRepository(httpClient, logger, Endpoint, Entity), IWishHttpClientRepository
{
    private const string Entity = "alert";
    private const string Endpoint = $"api/{Entity}s";

    public Task<WishResponse> CreateAsync(WishCreateRequest createRequest, CancellationToken cancellationToken)
    {
        return BaseCreateAsync<WishCreateRequest, WishResponse>(createRequest, cancellationToken);
    }

    public Task DeleteAsync(string alertId, CancellationToken cancellationToken)
    {
        return BaseDeleteAsync(alertId, cancellationToken);
    }

    public Task<WishResponse> GetByIdAsync(string alertId, CancellationToken cancellationToken)
    {
        return BaseGetByIdAsync<WishResponse>(alertId, cancellationToken);
    }

    public Task<PagedList<WishResponse>> GetPagedListAsync(WishQuery query, CancellationToken cancellationToken)
    {
        return BaseGetPagedListAsync<WishResponse>(query, cancellationToken);
    }

    public Task UpdateAsync(string alertId, WishUpdateRequest wishRequest, CancellationToken cancellationToken)
    {
        return BaseUpdateAsync(alertId, wishRequest, cancellationToken);
    }

    protected override List<KeyValuePair<string, StringValues>> AddSpecificQueryParameters(QueryParameters query)
    {
        var specificParams = new List<KeyValuePair<string, StringValues>>();

        if (query is WishQuery alertQuery)
        {
            if (!string.IsNullOrWhiteSpace(alertQuery.WithEmail))
                specificParams.Add(KeyValuePair.Create(nameof(alertQuery.WithEmail), new StringValues(alertQuery.WithEmail)));

            if (!string.IsNullOrWhiteSpace(alertQuery.WithSpiritually))
                specificParams.Add(KeyValuePair.Create(nameof(alertQuery.WithSpiritually), new StringValues(alertQuery.WithSpiritually)));
        }

        return specificParams;
    }
}
