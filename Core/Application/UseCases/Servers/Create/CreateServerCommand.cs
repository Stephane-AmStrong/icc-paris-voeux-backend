using Application.Abstractions.Handlers;
using DataTransfertObjects.Requests;
using DataTransfertObjects.Responses;

namespace Application.UseCases.Servers.Create;

public record CreateServerCommand(ServerCreateRequest Payload) : ICommand<ServerResponse>;
