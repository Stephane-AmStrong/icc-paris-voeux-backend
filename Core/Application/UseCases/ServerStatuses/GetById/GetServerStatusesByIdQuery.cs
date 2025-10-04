using Application.Abstractions.Handlers;
using DataTransfertObjects.Responses;

namespace Application.UseCases.ServerStatuses.GetById;

public record GetServerStatusByIdQuery(string Id) : IQuery<ServerStatusDetailedResponse>;
