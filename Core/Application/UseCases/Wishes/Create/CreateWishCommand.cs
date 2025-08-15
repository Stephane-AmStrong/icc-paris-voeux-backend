using Application.Abstractions.Handlers;
using DataTransfertObjects.Requests;
using DataTransfertObjects.Responses;

namespace Application.UseCases.Wishes.Create;

public record CreateWishCommand(WishCreateRequest Payload) : ICommand<WishResponse>;
