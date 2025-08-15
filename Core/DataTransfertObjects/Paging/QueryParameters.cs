#nullable enable
namespace DataTransfertObjects.Paging;

public record QueryParameters(string? SearchTerm, string? OrderBy, int? Page, int? PageSize);
