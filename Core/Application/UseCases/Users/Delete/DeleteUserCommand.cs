using Application.Abstractions.Handlers;

namespace Application.UseCases.Users.Delete;

public record DeleteUserCommand(string Id) : ICommand;
