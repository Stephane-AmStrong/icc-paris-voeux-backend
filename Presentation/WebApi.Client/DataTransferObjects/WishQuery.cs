#nullable enable
using WebApi.Client.Paging;

namespace WebApi.Client.DataTransferObjects;

public record WishQuery(
    string? WithEmail,
    string? WithSpiritually,
    string? SearchTerm,
    string? OrderBy,
    int? Page,
    int? PageSize
) : QueryParameters(SearchTerm, OrderBy, Page, PageSize);
