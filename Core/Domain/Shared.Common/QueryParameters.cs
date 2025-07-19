using System.Linq.Expressions;

namespace Domain.Shared.Common;

public abstract record QueryParameters<T>
{
    public Expression<Func<T, bool>>? Expression { get; set; }
    public int Page { get; set; } = 1;
    public string? OrderBy { get; set; }
    public string? SearchTerm { get; set; }

    private const int MaxPageSize = 50;
    private int _pageSize = 10;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = Math.Min(value, MaxPageSize);
    }
}