#nullable enable
namespace WebApi.Client.Paging;

public record QueryParameters(string? SearchTerm, string? OrderBy, int? Page, int? PageSize);
