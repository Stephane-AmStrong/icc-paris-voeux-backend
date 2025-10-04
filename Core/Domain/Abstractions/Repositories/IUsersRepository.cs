#nullable enable
using System.Linq.Expressions;
using Domain.Entities;
using Domain.Shared.Common;

namespace Domain.Abstractions.Repositories;

public interface IUsersRepository
{
    Task<PagedList<User>> GetPagedListByQueryAsync(BaseQuery<User> queryParameters, CancellationToken cancellationToken);
    Task<List<User>> FindByConditionAsync(Expression<Func<User, bool>> expression, CancellationToken cancellationToken);
    Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task CreateAsync(User user, CancellationToken cancellationToken);
    Task UpdateAsync(User user, CancellationToken cancellationToken);
    Task DeleteAsync(User user, CancellationToken cancellationToken);
}
