using Application.Abstractions.Handlers;

namespace Application.UseCases.Wishes.Delete;

public record DeleteWishCommand(string Id) : ICommand;