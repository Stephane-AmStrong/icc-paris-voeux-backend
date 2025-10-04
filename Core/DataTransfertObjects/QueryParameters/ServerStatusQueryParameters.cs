#nullable enable
using DataTransfertObjects.Enumerations;

namespace DataTransfertObjects.QueryParameters;

public record ServerStatusQueryParameters(
    string? WithServerId = null,
    ServerStatus? OfStatus = null,
    DateTime? RecordedBefore = null,
    DateTime? RecordedAfter = null,
    string? SearchTerm = null,
    string? OrderBy = null,
    int? Page = null,
    int? PageSize = null
) : Paging.QueryParameters(SearchTerm, OrderBy, Page, PageSize);
