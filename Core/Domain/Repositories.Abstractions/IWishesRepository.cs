#nullable enable
using System.Linq.Expressions;

using Domain.Entities;
using Domain.Shared.Common;

namespace Domain.Repositories.Abstractions;

public interface IWishesRepository
{
    Task<PagedList<Wish>> GetPagedListByQueryAsync(BaseQueryParameters<Wish> baseQueryParameters, CancellationToken cancellationToken);
    Task<List<Wish>> FindByConditionAsync(Expression<Func<Wish, bool>> expression, CancellationToken cancellationToken);
    Task<Wish?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task CreateAsync(Wish wish, CancellationToken cancellationToken);
    Task UpdateAsync(Wish wish, CancellationToken cancellationToken);
    Task DeleteAsync(Wish wish, CancellationToken cancellationToken);
}
