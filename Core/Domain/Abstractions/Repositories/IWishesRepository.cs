#nullable enable
using System.Linq.Expressions;
using Domain.Entities;
using Domain.Shared.Common;

namespace Domain.Abstractions.Repositories;

public interface IWishesRepository
{
    Task<PagedList<Wish>> GetPagedListByQueryAsync(BaseQuery<Wish> queryParameters, CancellationToken cancellationToken);
    Task<List<Wish>> FindByConditionAsync(Expression<Func<Wish, bool>> expression, CancellationToken cancellationToken);
    Task<Wish?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task CreateAsync(Wish wish, CancellationToken cancellationToken);
    Task UpdateAsync(Wish wish, CancellationToken cancellationToken);
    Task DeleteAsync(Wish wish, CancellationToken cancellationToken);
}
