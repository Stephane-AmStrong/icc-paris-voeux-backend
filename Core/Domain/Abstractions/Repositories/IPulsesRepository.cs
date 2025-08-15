#nullable enable
using System.Linq.Expressions;
using Domain.Entities;
using Domain.Shared.Common;

namespace Domain.Abstractions.Repositories;

public interface IPulsesRepository
{
    Task<PagedList<Pulse>> GetPagedListByQueryAsync(BaseQuery<Pulse> queryParameters, CancellationToken cancellationToken);
    Task<List<Pulse>> FindByConditionAsync(Expression<Func<Pulse, bool>> expression, CancellationToken cancellationToken);
    Task<List<Pulse>> GetLatestPulsesByUserIdsAsync(string[] userIds, CancellationToken cancellationToken);
    Task<Pulse?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task CreateAsync(Pulse pulse, CancellationToken cancellationToken);
    Task UpdateAsync(Pulse pulse, CancellationToken cancellationToken);
}
