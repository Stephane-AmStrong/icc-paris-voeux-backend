using System.Text.Json;
using Application.Abstractions.Handlers;
using Application.UseCases.Wishes.Create;
using Application.UseCases.Wishes.Delete;
using Application.UseCases.Wishes.GetById;
using Application.UseCases.Wishes.GetByQuery;
using Application.UseCases.Wishes.Update;
using Domain.Shared.Common;
using WebApi.Extensions;

namespace WebApi.Endpoints;
public static class WishesEndpoints
{
    public static void MapWishesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/wishes")
            .WithTags("Wishes");

        group.MapGet("/", GetByQueryParameters)
            .Produces<IList<WishResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{id}", GetWishById)
            .Produces<WishResponse>(StatusCodes.Status200OK)
            .WithName(nameof(GetWishById));

        group.MapPost("/", CreateWish)
            .WithRequestValidation<WishCreateRequest>()
            .Produces<WishResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest);

        group.MapDelete("/{id}", DeleteWish)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{id}", UpdateWish)
            .WithRequestValidation<WishUpdateRequest>()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
    }

    // GET /api/wishes
    private static  async Task<IResult> GetByQueryParameters(IQueryHandler<GetWishByQuery, PagedList<WishResponse>> handler, [AsParameters] WishQuery queryParameters, HttpResponse response, CancellationToken cancellationToken)
    {
        var wishesResponse = await handler.HandleAsync(new GetWishByQuery(queryParameters), cancellationToken);
        
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
        await handler.HandleAsync(new DeleteWishCommand { Id = id }, cancellationToken);
        return Results.NoContent();
    }

    // PUT /api/wishes/{id}
    private static async Task<IResult> UpdateWish(ICommandHandler<UpdateWishCommand> handler, string id, WishUpdateRequest wishRequest, CancellationToken cancellationToken)
    {
        await handler.HandleAsync(new UpdateWishCommand(id, wishRequest), cancellationToken);;
        return Results.NoContent();
    }
}
