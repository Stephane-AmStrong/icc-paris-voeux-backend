#nullable enable
using Application.UseCases.Wishes.Create;
using Application.UseCases.Wishes.GetById;
using Application.UseCases.Wishes.GetByQuery;
using Application.UseCases.Wishes.Update;
using Domain.Shared.Common;

namespace Application.Services.Abstractions;

public interface IWishesService
{
    Task<PagedList<WishResponse>> GetPagedListByQueryAsync(WishQuery queryParameters, CancellationToken cancellationToken);
    Task<WishDetailedResponse?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<WishResponse> CreateAsync(WishCreateRequest wishRequest, CancellationToken cancellationToken);
    Task UpdateAsync(string id, WishUpdateRequest wishRequest, CancellationToken cancellationToken);
    Task DeleteAsync(string id, CancellationToken cancellationToken);
}
