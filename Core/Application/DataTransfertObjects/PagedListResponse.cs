using Domain.Shared.Common;

namespace Application.DataTransfertObjects;

public class PagedListResponse<T>(List<T> pagedList, MetaData metaData)
{
    public PagedListResponse() : this([], new MetaData()) { }

    public MetaData MetaData { get; set; } = new()
    {
        TotalCount = metaData.TotalCount,
        PageSize = metaData.PageSize,
        CurrentPage = metaData.CurrentPage,
        TotalPages = metaData.TotalPages
    };

    public List<T> PagedList { get; set; } = pagedList;
}