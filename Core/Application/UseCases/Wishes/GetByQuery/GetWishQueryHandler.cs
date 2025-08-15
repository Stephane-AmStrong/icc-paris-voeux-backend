#nullable enable
using Application.Abstractions.Handlers;
using Application.Abstractions.Services;
using Domain.Shared.Common;
using DataTransfertObjects.Responses;

namespace Application.UseCases.Wishes.GetByQuery;

public class GetWishQueryHandler(IWishesService wishesService) : IQueryHandler<GetWishQuery, PagedList<WishResponse>>
{
    public Task<PagedList<WishResponse>> HandleAsync(GetWishQuery query, CancellationToken cancellationToken)
    {
        return wishesService.GetPagedListByQueryAsync(new WishQuery(query.Parameters), cancellationToken);
    }
}
