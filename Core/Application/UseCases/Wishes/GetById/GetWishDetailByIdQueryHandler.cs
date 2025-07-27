using Application.Abstractions.Handlers;
using Application.Abstractions.Services;

namespace Application.UseCases.Wishes.GetById;

public class GetWishByIdQueryHandler(IWishesService wishesService) : IQueryHandler<GetWishByIdQuery, WishDetailedResponse?>
{
    public async Task<WishDetailedResponse?> HandleAsync(GetWishByIdQuery query, CancellationToken cancellationToken)
    {
        return await wishesService.GetByIdAsync(query.Id, cancellationToken);
    }
}
