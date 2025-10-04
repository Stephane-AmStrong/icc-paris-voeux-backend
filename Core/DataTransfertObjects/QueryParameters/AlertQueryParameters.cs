#nullable enable
using DataTransfertObjects.Enumerations;

namespace DataTransfertObjects.QueryParameters;

public record AlertQueryParameters(
    string? WithServerId = null,
    AlertType? OfType = null,
    AlertSeverity? OfSeverity = null,
    DateTime? OccurredBefore = null,
    DateTime? OccurredAfter = null,
    string? SearchTerm = null,
    string? OrderBy = null,
    int? Page = null,
    int? PageSize = null
) : Paging.QueryParameters(SearchTerm, OrderBy, Page, PageSize);
