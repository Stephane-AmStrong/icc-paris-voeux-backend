#nullable enable
using System.Linq.Expressions;
using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Shared.Common;
using MongoDB.Driver;

namespace Persistence.Repository;

public sealed class WishesRepository(IMongoDatabase database) : RepositoryBase<Wish>(database, DataTables.Wishes), IWishesRepository
{
    public Task<PagedList<Wish>> GetPagedListByQueryAsync(BaseQuery<Wish> queryParameters, CancellationToken cancellationToken)
    {
        return BaseQueryWithFiltersAsync(queryParameters, cancellationToken);
    }

    public Task<List<Wish>> FindByConditionAsync(Expression<Func<Wish, bool>> expression, CancellationToken cancellationToken)
    {
        return BaseFindByConditionAsync(expression, cancellationToken);
    }

    public async Task<Wish?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var wishes = await BaseFindByConditionAsync(wish => wish.Id == id, cancellationToken);
        return wishes.FirstOrDefault();
    }

    public async Task CreateAsync(Wish wish, CancellationToken cancellationToken)
    {
        await BaseCreateAsync(wish, cancellationToken);
    }

    public Task UpdateAsync(Wish wish, CancellationToken cancellationToken)
    {
        return BaseUpdateAsync(wish, cancellationToken);
    }

    public Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        return BaseDeleteAsync(id, cancellationToken);
    }
}
