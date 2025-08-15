using Application.Abstractions.Handlers;
using Application.Abstractions.Services;
using DataTransfertObjects.Responses;

namespace Application.UseCases.Wishes.GetById;

public class GetWishByIdQueryHandler(IWishesService wishesService) : IQueryHandler<GetWishByIdQuery, WishDetailedResponse>
{
    public Task<WishDetailedResponse> HandleAsync(GetWishByIdQuery query, CancellationToken cancellationToken)
    {
        return wishesService.GetByIdAsync(query.Id, cancellationToken);
    }
}
