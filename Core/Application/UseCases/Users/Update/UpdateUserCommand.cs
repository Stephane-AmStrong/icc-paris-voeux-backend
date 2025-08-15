using Application.Abstractions.Handlers;
using DataTransfertObjects.Requests;

namespace Application.UseCases.Users.Update;

public record UpdateUserCommand(string Id, UserUpdateRequest Payload) : ICommand;
