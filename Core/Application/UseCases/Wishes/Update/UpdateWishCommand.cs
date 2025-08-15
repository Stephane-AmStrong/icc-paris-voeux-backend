using Application.Abstractions.Handlers;
using DataTransfertObjects.Requests;

namespace Application.UseCases.Wishes.Update;

public record UpdateWishCommand(string Id, WishUpdateRequest Payload) : ICommand;
