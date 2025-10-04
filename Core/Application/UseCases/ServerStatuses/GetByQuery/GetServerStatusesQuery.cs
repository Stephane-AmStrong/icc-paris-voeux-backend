#nullable enable
using Application.Abstractions.Handlers;
using DataTransfertObjects.QueryParameters;
using DataTransfertObjects.Responses;
using Domain.Shared.Common;

namespace Application.UseCases.ServerStatuses.GetByQuery;

public record GetServerStatusQuery(ServerStatusQueryParameters Parameters) : IQuery<PagedList<ServerStatusResponse>>;
