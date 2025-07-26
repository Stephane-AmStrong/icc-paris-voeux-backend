using Application.Abstractions.Handlers;
using Application.Abstractions.Services;
using Domain.Shared.Common;

namespace Application.UseCases.Wishes.GetByQuery;

public class GetWishByQueryHandler(IWishesService wishesService) : IQueryHandler<GetWishByQuery, PagedList<WishResponse>>
{
    public async Task<PagedList<WishResponse>> HandleAsync(GetWishByQuery query, CancellationToken cancellationToken)
    {
        return await wishesService.GetPagedListByQueryAsync(query.Payload, cancellationToken);
    }
}
