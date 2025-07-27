using Application.Abstractions.Handlers;
using Application.UseCases.Wishes.GetByQuery;

namespace Application.UseCases.Wishes.Create;

public record CreateWishCommand(WishCreateRequest Payload) : ICommand<WishResponse>;
