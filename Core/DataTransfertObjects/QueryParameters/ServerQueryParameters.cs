#nullable enable

namespace DataTransfertObjects.QueryParameters;

public record ServerQueryParameters(
    string? WithHostName = null,
    string? WithAppName = null,
    string? WithVersion = null,
    string? SearchTerm = null,
    string? OrderBy = null,
    int? Page = null,
    int? PageSize = null
) : Paging.QueryParameters(SearchTerm, OrderBy, Page, PageSize);

