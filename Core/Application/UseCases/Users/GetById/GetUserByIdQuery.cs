using Application.Abstractions.Handlers;
using DataTransfertObjects.Responses;

namespace Application.UseCases.Users.GetById;

public record GetUserByIdQuery(string Id) : IQuery<UserDetailedResponse>;
