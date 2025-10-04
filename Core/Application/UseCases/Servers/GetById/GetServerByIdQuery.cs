using Application.Abstractions.Handlers;
using DataTransfertObjects.Responses;

namespace Application.UseCases.Servers.GetById;

public record GetServerByIdQuery(string Id) : IQuery<ServerDetailedResponse>;
