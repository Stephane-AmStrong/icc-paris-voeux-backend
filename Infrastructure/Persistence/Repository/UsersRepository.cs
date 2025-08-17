#nullable enable
using System.Linq.Expressions;
using Domain.Abstractions.Events;
using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Events;
using Domain.Shared.Common;
using MongoDB.Driver;

namespace Persistence.Repository;

public sealed class UsersRepository(IMongoDatabase database, IEventsDispatcher eventsDispatcher) : RepositoryBase<User>(database, eventsDispatcher, DataTables.Users), IUsersRepository
{
    public Task<PagedList<User>> GetPagedListByQueryAsync(BaseQuery<User> queryParameters, CancellationToken cancellationToken)
    {
        return BaseQueryWithFiltersAsync(queryParameters, cancellationToken);
    }

    public Task<List<User>> FindByConditionAsync(Expression<Func<User, bool>> expression, CancellationToken cancellationToken)
    {
        return BaseFindByConditionAsync(expression, cancellationToken);
    }

    public async Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var users = await BaseFindByConditionAsync(user => user.Id == id, cancellationToken);
        return users.FirstOrDefault();
    }

    public async Task CreateAsync(User user, CancellationToken cancellationToken)
    {
        user.Raise(new UserCreatedEvent(user));
        await BaseCreateAsync(user, cancellationToken);
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken)
    {
        user.Raise(new UserUpdatedEvent(user));
        return BaseUpdateAsync(user, cancellationToken);
    }

    public Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        return BaseDeleteAsync(id, cancellationToken);
    }
}
