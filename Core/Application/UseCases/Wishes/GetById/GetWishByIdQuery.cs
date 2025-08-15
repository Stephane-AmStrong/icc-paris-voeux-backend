using Application.Abstractions.Handlers;
using DataTransfertObjects.Responses;

namespace Application.UseCases.Wishes.GetById;

public record GetWishByIdQuery(string Id) : IQuery<WishDetailedResponse>;
