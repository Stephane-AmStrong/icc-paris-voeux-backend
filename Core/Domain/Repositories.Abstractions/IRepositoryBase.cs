using System.Linq.Expressions;
using Domain.Shared.Common;

namespace Domain.Repositories.Abstractions;

public interface IRepositoryBase<T>
{
    Task<List<T>> BaseFindByConditionAsync(QueryParameters<T> parameters, CancellationToken cancellationToken);
    Task<List<T>> BaseFindByConditionAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken);
    Task BaseCreateAsync(T entity, CancellationToken cancellationToken);
    Task BaseUpdateAsync(T entity, CancellationToken cancellationToken);
    Task BaseDeleteAsync(T entity, CancellationToken cancellationToken);
}