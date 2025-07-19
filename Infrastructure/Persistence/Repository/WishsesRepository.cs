#nullable enable
using System.Linq.Expressions;

using Domain.Entities;
using Domain.Repositories.Abstractions;
using Domain.Shared.Common;
using MongoDB.Driver;

namespace Persistence.Repository;

public sealed class WishsesRepository(IMongoDatabase database) : RepositoryBase<Wish>(database, "WT_Clients"), IWishesRepository
{
    public async Task CreateAsync(Wish wish, CancellationToken cancellationToken)
    {
        await BaseCreateAsync(wish, cancellationToken);
    }

    public Task DeleteAsync(Wish wish, CancellationToken cancellationToken)
    {
        return BaseDeleteAsync(wish, cancellationToken);
    }

    public Task<List<Wish>> GetAllAsync(CancellationToken cancellationToken)
    {
        return BaseFindByConditionAsync(client => true, cancellationToken);
    }

    public async Task<Wish?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var clients = await BaseFindByConditionAsync(client => client.Id == id, cancellationToken);
        return clients.FirstOrDefault();
    }

    public Task<List<Wish>> GetByConditionAsync(QueryParameters<Wish> queryParameters, CancellationToken cancellationToken)
    {
        return BaseFindByConditionAsync(queryParameters, cancellationToken);
    }

    public Task UpdateAsync(Wish wish, CancellationToken cancellationToken)
    {
        return BaseUpdateAsync(wish, cancellationToken);
    }
}
