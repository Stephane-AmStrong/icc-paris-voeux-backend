#nullable enable
using System.Linq.Expressions;

namespace Domain.Shared.Common;

public abstract record BaseQueryParameters<T>
{
    private Expression<Func<T, bool>>? _filterExpression { get; set; }
    public int? Page { get; set; } = 1;
    public string? OrderBy { get; set; }
    public string? SearchTerm { get; set; }

    private const int MaxPageSize = 50;
    private int? _pageSize;

    public int? PageSize
    {
        get => _pageSize ?? 10;
        set => _pageSize = Math.Min(value ?? 10, MaxPageSize);
    }

    protected void SetFilterExpression(Expression<Func<T, bool>> filterExpression) => _filterExpression = filterExpression;
    public Expression<Func<T, bool>>? GetFilterExpression() => _filterExpression;
}
