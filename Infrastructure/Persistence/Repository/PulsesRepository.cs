#nullable enable
using System.Linq.Expressions;
using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Shared.Common;
using MongoDB.Driver;

namespace Persistence.Repository;

public sealed class PulsesRepository(IMongoDatabase database) : RepositoryBase<Pulse>(database, DataTables.Pulses), IPulsesRepository
{
    public Task<PagedList<Pulse>> GetPagedListByQueryAsync(BaseQuery<Pulse> queryParameters, CancellationToken cancellationToken)
    {
        return BaseQueryWithFiltersAsync(queryParameters, cancellationToken);
    }

    public Task<List<Pulse>> FindByConditionAsync(Expression<Func<Pulse, bool>> expression, CancellationToken cancellationToken)
    {
        return BaseFindByConditionAsync(expression, cancellationToken);
    }

    public async Task<List<Pulse>> GetLatestPulsesByUserIdsAsync(string[] userIds, CancellationToken cancellationToken)
    {
        return await BaseGetLatestByGroupAsync(
            pulse => userIds.Contains(pulse.UserId),
            pulse => pulse.UserId,
            pulse => pulse.RecordedAt,
            cancellationToken);
    }

    public async Task<Pulse?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var pulses = await BaseFindByConditionAsync(pulse => pulse.Id == id, cancellationToken);
        return pulses.FirstOrDefault();
    }

    public async Task CreateAsync(Pulse pulse, CancellationToken cancellationToken)
    {
        await BaseCreateAsync(pulse, cancellationToken);
    }

    public Task UpdateAsync(Pulse pulse, CancellationToken cancellationToken)
    {
        return BaseUpdateAsync(pulse, cancellationToken);
    }
}
