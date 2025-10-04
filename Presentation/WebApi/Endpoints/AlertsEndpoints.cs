using System.Text.Json;
using Application.Abstractions.Handlers;
using Application.UseCases.Alerts.CreateOrIncrement;
using Application.UseCases.Alerts.Delete;
using Application.UseCases.Alerts.GetById;
using Application.UseCases.Alerts.GetByQuery;
using Application.UseCases.Alerts.Update;
using DataTransfertObjects.QueryParameters;
using DataTransfertObjects.Requests;
using DataTransfertObjects.Responses;
using Domain.Shared.Common;
using WebApi.Models;

namespace WebApi.Endpoints;
public static class AlertsEndpoints
{
    public static void MapAlertsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/alerts")
            .WithTags("Alerts");

        group.MapGet("/", GetByQueryParameters)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<IList<AlertResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{id}", GetAlertById)
            .Produces<AlertDetailedResponse>(StatusCodes.Status200OK)
            .WithName(nameof(GetAlertById));

        group.MapPost("/", CreateOrIncrementAlert)
            .Produces<AlertDetailedResponse>(StatusCodes.Status201Created)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapDelete("/{id}", DeleteAlert)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{id}", UpdateAlert)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
    }

    // GET /api/alerts
    private static  async Task<IResult> GetByQueryParameters(IQueryHandler<GetAlertQuery, PagedList<AlertResponse>> handler, [AsParameters] AlertQueryParameters queryParameters, HttpResponse response, CancellationToken cancellationToken)
    {
        var alertsResponse = await handler.HandleAsync(new GetAlertQuery(queryParameters), cancellationToken);

        response.Headers.Append("X-Pagination", JsonSerializer.Serialize(alertsResponse.MetaData));

        return Results.Ok(alertsResponse);
    }

    // GET /api/alerts/{id}
    private static async Task<IResult> GetAlertById(IQueryHandler<GetAlertByIdQuery, AlertDetailedResponse?> handler, string id, CancellationToken cancellationToken)
    {
        var alertResponse = await handler.HandleAsync(new GetAlertByIdQuery(id), cancellationToken);
        return Results.Ok(alertResponse);
    }

    // POST /api/alerts
    private static async Task<IResult> CreateOrIncrementAlert(ICommandHandler<CreateOrIncrementAlertCommand, AlertResponse> handler, AlertCreateOrIncrementRequest alertRequest, CancellationToken cancellationToken)
    {
        var alertResponse = await handler.HandleAsync(new CreateOrIncrementAlertCommand(alertRequest), cancellationToken);
        return Results.CreatedAtRoute(nameof(GetAlertById), new { id = alertResponse.Id }, alertResponse);
    }

    // DELETE /api/alerts/{id}
    private static async Task<IResult> DeleteAlert(ICommandHandler<DeleteAlertCommand> handler, string id, CancellationToken cancellationToken)
    {
        await handler.HandleAsync(new DeleteAlertCommand(id), cancellationToken);
        return Results.NoContent();
    }

    // PUT /api/alerts/{id}
    private static async Task<IResult> UpdateAlert(ICommandHandler<UpdateAlertCommand> handler, string id, AlertUpdateRequest alertRequest, CancellationToken cancellationToken)
    {
        await handler.HandleAsync(new UpdateAlertCommand(id, alertRequest), cancellationToken);
        return Results.NoContent();
    }
}
