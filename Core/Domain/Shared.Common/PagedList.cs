namespace Domain.Shared.Common;

public class PagedList<T> : List<T>
{
    public MetaData MetaData { get; set; }

    public PagedList(List<T> items, long count, int pageNumber, int pageSize)
    {
        MetaData = new MetaData(pageNumber, pageSize, count);

        AddRange(items);
    }

    public PagedList(List<T> items, MetaData metaData)
    {
        MetaData = metaData;
        AddRange(items);
    }

    public static PagedList<T> ToPagedList(IEnumerable<T> source, int pageNumber, int pageSize)
    {
        var count = source.Count();
        var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}
