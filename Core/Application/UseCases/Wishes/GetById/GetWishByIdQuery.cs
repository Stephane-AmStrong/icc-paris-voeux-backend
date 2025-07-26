#nullable enable
using Application.Abstractions.Handlers;

namespace Application.UseCases.Wishes.GetById;

public record GetWishByIdQuery(string Id) : IQuery<WishDetailedResponse?>;
