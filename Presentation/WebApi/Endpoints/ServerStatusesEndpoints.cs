using System.Text.Json;
using Application.Abstractions.Handlers;
using Application.UseCases.ServerStatuses.Create;
using Application.UseCases.ServerStatuses.GetById;
using Application.UseCases.ServerStatuses.GetByQuery;
using DataTransfertObjects.QueryParameters;
using DataTransfertObjects.Requests;
using DataTransfertObjects.Responses;
using Domain.Shared.Common;
using WebApi.Models;

namespace WebApi.Endpoints;
public static class ServerStatusesEndpoints
{
    public static void MapServerStatusesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/serverStatuses")
            .WithTags("ServerStatuses");

        group.MapGet("/", GetByQueryParameters)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<IList<ServerStatusResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{id}", GetServerStatusById)
            .Produces<ServerStatusDetailedResponse>(StatusCodes.Status200OK)
            .WithName(nameof(GetServerStatusById));

        group.MapPost("/", CreateServerStatus)
            .Produces<ServerStatusDetailedResponse>(StatusCodes.Status201Created)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest);
    }

    // GET /api/serverStatuses
    private static  async Task<IResult> GetByQueryParameters(IQueryHandler<GetServerStatusQuery, PagedList<ServerStatusResponse>> handler, [AsParameters] ServerStatusQueryParameters queryParameters, HttpResponse response, CancellationToken cancellationToken)
    {
        var serverStatusesResponse = await handler.HandleAsync(new GetServerStatusQuery(queryParameters), cancellationToken);

        response.Headers.Append("X-Pagination", JsonSerializer.Serialize(serverStatusesResponse.MetaData));

        return Results.Ok(serverStatusesResponse);
    }

    // GET /api/serverStatuses/{id}
    private static async Task<IResult> GetServerStatusById(IQueryHandler<GetServerStatusByIdQuery, ServerStatusDetailedResponse?> handler, string id, CancellationToken cancellationToken)
    {
        var serverStatusResponse = await handler.HandleAsync(new GetServerStatusByIdQuery(id), cancellationToken);
        return Results.Ok(serverStatusResponse);
    }

    // POST /api/serverStatuses
    private static async Task<IResult> CreateServerStatus(ICommandHandler<CreateServerStatusCommand, ServerStatusResponse> handler, ServerStatusCreateRequest serverStatusRequest, CancellationToken cancellationToken)
    {
        var serverStatusResponse = await handler.HandleAsync(new CreateServerStatusCommand(serverStatusRequest), cancellationToken);
        return Results.CreatedAtRoute(nameof(GetServerStatusById), new { id = serverStatusResponse.Id }, serverStatusResponse);
    }
}
