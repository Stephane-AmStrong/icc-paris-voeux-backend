#nullable enable
using DataTransfertObjects.Enumerations;

namespace DataTransfertObjects.QueryParameters;

public record WishQueryParameters(
    string? WithUserId = null,
    string? WithTitle = null,
    WishType? OfType = null,
    DateTime? CreatedBefore = null,
    DateTime? CreatedAfter = null,
    DateTime? FulfilledBefore = null,
    DateTime? FulfilledAfter = null,
    string? SearchTerm = null,
    string? OrderBy = null,
    int? Page = null,
    int? PageSize = null
) : Paging.QueryParameters(SearchTerm, OrderBy, Page, PageSize);