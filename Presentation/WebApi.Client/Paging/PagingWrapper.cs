namespace WebApi.Client.Paging;

public record PagedList<T>(List<T> Data, PagingMetadata MetaData);

public abstract record PagingMetadata(int CurrentPage, int PageSize, long TotalCount)
{
    public int TotalPages => TotalCount == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);

    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
}
