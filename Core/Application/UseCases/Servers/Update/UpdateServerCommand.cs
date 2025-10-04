using Application.Abstractions.Handlers;
using DataTransfertObjects.Requests;

namespace Application.UseCases.Servers.Update;

public record UpdateServerCommand(string Id, ServerUpdateRequest Payload) : ICommand;
