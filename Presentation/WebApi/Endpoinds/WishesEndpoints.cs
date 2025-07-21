using System.Text.Json;
using Application.DataTransfertObjects;
using Application.Services.Abstractions;
using WebApi.Extensions;

namespace WebApi.Endpoinds;
public static class WishesEndpoints
{
    public static void MapWishesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/wishes")
            .WithTags("Wishes");

        group.MapGet("/", GetByQueryParameters)
            .Produces<IList<WishResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{id}", GetWishById)
            .Produces<WishResponse>(StatusCodes.Status200OK);

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
    private static  async Task<IResult> GetByQueryParameters(IWishesService wishesService,[AsParameters] WishQueryParameters queryParameters, HttpResponse response, CancellationToken cancellationToken)
    {
        var wishesResponse = await wishesService.GetPagedListByQueryAsync(queryParameters, cancellationToken);
        
        response.Headers.Append("X-Pagination", JsonSerializer.Serialize(wishesResponse.MetaData));

        return Results.Ok(wishesResponse.PagedList);
    }

    // GET /api/wishes/{id}
    private static async Task<IResult> GetWishById(IWishesService wishesService, string id, CancellationToken cancellationToken)
    {
        var wishResponse = await wishesService.GetByIdAsync(id, cancellationToken);
        return Results.Ok(wishResponse);
    }

    // POST /api/wishes
    private static async Task<IResult> CreateWish(IWishesService wishesService, WishCreateRequest wishRequest, CancellationToken cancellationToken)
    {
        var wishResponse = await wishesService.CreateAsync(wishRequest, cancellationToken);
        return Results.Created(wishResponse.Id.ToString(), wishResponse);
    }

    // DELETE /api/wishes/{id}
    private static async Task<IResult> DeleteWish(IWishesService wishesService, string id, CancellationToken cancellationToken)
    {
        await wishesService.DeleteAsync(id, cancellationToken);
        return Results.NoContent();
    }

    // PUT /api/wishes/{id}
    private static async Task<IResult> UpdateWish(IWishesService wishesService, string id, WishUpdateRequest wishRequest, CancellationToken cancellationToken)
    {
        await wishesService.UpdateAsync(id, wishRequest, cancellationToken);
        return Results.NoContent();
    }
}
