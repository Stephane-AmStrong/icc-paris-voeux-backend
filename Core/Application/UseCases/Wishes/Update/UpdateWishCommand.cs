using Application.Abstractions.Handlers;

namespace Application.UseCases.Wishes.Update;

public record UpdateWishCommand(string Id, WishUpdateRequest Payload) : ICommand;