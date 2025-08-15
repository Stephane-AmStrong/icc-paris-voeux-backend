using Application.Abstractions.Handlers;
using DataTransfertObjects.Requests;
using DataTransfertObjects.Responses;

namespace Application.UseCases.Users.Create;

public record CreateUserCommand(UserCreateRequest Payload) : ICommand<UserResponse>;
