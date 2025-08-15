using System.Linq.Expressions;
using Domain.Entities;
using Domain.Shared.Common;

namespace Domain.Abstractions.Repositories;

public interface IRepositoryBase<T> where T : IBaseEntity
{
    Task<PagedList<T>> BaseQueryWithFiltersAsync(BaseQuery<T> queryParameters, CancellationToken cancellationToken);
    Task<List<T>> BaseFindByConditionAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken);
    Task BaseCreateAsync(T entity, CancellationToken cancellationToken);
    Task BaseUpdateAsync(T entity, CancellationToken cancellationToken);
    Task BaseDeleteAsync(string id, CancellationToken cancellationToken);
}
