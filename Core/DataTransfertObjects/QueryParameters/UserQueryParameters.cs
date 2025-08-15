#nullable enable

namespace DataTransfertObjects.QueryParameters;

public record UserQueryParameters(
    string? WithEmail = null,
    string? WithLastName = null,
    string? WithFirstName = null,
    string? SearchTerm = null,
    string? OrderBy = null,
    int? Page = null,
    int? PageSize = null
) : Paging.QueryParameters(SearchTerm, OrderBy, Page, PageSize);