#nullable enable
using Application.Abstractions.Handlers;
using Domain.Shared.Common;

namespace Application.UseCases.Wishes.GetByQuery;

public record GetWishByQuery(WishQuery Payload) : IQuery<PagedList<WishResponse>>;
