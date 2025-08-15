#nullable enable
using Application.Abstractions.Handlers;
using Domain.Shared.Common;
using DataTransfertObjects.QueryParameters;
using DataTransfertObjects.Responses;

namespace Application.UseCases.Wishes.GetByQuery;

public record GetWishQuery(WishQueryParameters Parameters) : IQuery<PagedList<WishResponse>>;
