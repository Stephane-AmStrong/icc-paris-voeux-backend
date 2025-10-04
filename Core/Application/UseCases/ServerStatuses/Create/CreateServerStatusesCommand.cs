using Application.Abstractions.Handlers;
using DataTransfertObjects.Requests;
using DataTransfertObjects.Responses;

namespace Application.UseCases.ServerStatuses.Create;

public record CreateServerStatusCommand(ServerStatusCreateRequest Payload) : ICommand<ServerStatusResponse>;
