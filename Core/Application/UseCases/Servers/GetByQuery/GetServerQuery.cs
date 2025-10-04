#nullable enable
using Application.Abstractions.Handlers;
using DataTransfertObjects.QueryParameters;
using DataTransfertObjects.Responses;
using Domain.Shared.Common;

namespace Application.UseCases.Servers.GetByQuery;

public record GetServerQuery(ServerQueryParameters Parameters) : IQuery<PagedList<ServerResponse>>;
