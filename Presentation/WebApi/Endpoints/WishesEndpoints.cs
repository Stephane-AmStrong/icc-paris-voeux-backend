using System.Text.Json;
using Application.Abstractions.Handlers;
using Application.UseCases.Wishes.Create;
using Application.UseCases.Wishes.Delete;
using Application.UseCases.Wishes.GetById;
using Application.UseCases.Wishes.GetByQuery;
using Application.UseCases.Wishes.Update;
using Domain.Shared.Common;
using DataTransfertObjects.QueryParameters;
using DataTransfertObjects.Requests;
using DataTransfertObjects.Responses;
using WebApi.Models;

namespace WebApi.Endpoints;
public static class WishesEndpoints
{
    public static void MapWishesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/wishes")
            .WithTags("Wishes");

        group.MapGet("/", GetByQueryParameters)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<IList<WishResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{id}", GetWishById)
            .Produces<WishDetailedResponse>(StatusCodes.Status200OK)
            .WithName(nameof(GetWishById));

        group.MapPost("/", CreateWish)
            .Produces<WishDetailedResponse>(StatusCodes.Status201Created)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapDelete("/{id}", DeleteWish)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{id}", UpdateWish)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
    }

    // GET /api/wishes
    private static  async Task<IResult> GetByQueryParameters(IQueryHandler<GetWishQuery, PagedList<WishResponse>> handler, [AsParameters] WishQueryParameters queryParameters, HttpResponse response, CancellationToken cancellationToken)
    {
        var wishesResponse = await handler.HandleAsync(new GetWishQuery(queryParameters), cancellationToken);

        response.Headers.Append("X-Pagination", JsonSerializer.Serialize(wishesResponse.MetaData));

        return Results.Ok(wishesResponse);
    }

    // GET /api/wishes/{id}
    private static async Task<IResult> GetWishById(IQueryHandler<GetWishByIdQuery, WishDetailedResponse?> handler, string id, CancellationToken cancellationToken)
    {
        var wishResponse = await handler.HandleAsync(new GetWishByIdQuery(id), cancellationToken);
        return Results.Ok(wishResponse);
    }

    // POST /api/wishes
    private static async Task<IResult> CreateWish(ICommandHandler<CreateWishCommand, WishResponse> handler, WishCreateRequest wishRequest, CancellationToken cancellationToken)
    {
        var wishResponse = await handler.HandleAsync(new CreateWishCommand(wishRequest), cancellationToken);
        return Results.CreatedAtRoute(nameof(GetWishById), new { id = wishResponse.Id }, wishResponse);
    }

    // DELETE /api/wishes/{id}
    private static async Task<IResult> DeleteWish(ICommandHandler<DeleteWishCommand> handler, string id, CancellationToken cancellationToken)
    {
        await handler.HandleAsync(new DeleteWishCommand(id), cancellationToken);
        return Results.NoContent();
    }

    // PUT /api/wishes/{id}
    private static async Task<IResult> UpdateWish(ICommandHandler<UpdateWishCommand> handler, string id, WishUpdateRequest wishRequest, CancellationToken cancellationToken)
    {
        await handler.HandleAsync(new UpdateWishCommand(id, wishRequest), cancellationToken);
        return Results.NoContent();
    }
}
