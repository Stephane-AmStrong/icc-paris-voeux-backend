#nullable enable
using Application.DataTransfertObjects;
using Application.Services.Abstractions;

using Domain.Entities;
using Domain.Errors;
using Domain.Repositories.Abstractions;

using Mapster;
using Microsoft.Extensions.Logging;

namespace Services;

public sealed class WishesService(
    IWishesRepository wishesRepository,
    ILogger<WishesService> logger
    ) : IWishesService
{
    public async Task<WishResponse> CreateAsync(WishCreateRequest wishRequest, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating new wish with email: {WishEmail}", wishRequest.Email);
        var wish = wishRequest.Adapt<Wish>();

        await wishesRepository.CreateAsync(wish, cancellationToken);

        logger.LogInformation("Successfully created wish with ID: {WishId}", wish.Id);
        return wish.Adapt<WishResponse>();
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting wish with ID: {WishId}", id);
        var wish = await wishesRepository.GetByIdAsync(id, cancellationToken) ?? throw new WishNotFoundException(id);

        logger.LogInformation("Successfully deleted wish with ID: {WishId}", id);
        await wishesRepository.DeleteAsync(wish, cancellationToken);
    }

    public async Task<PagedListResponse<WishResponse>> GetPagedListByQueryAsync(WishQueryParameters queryParameters,CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting wishes with query parameters: {@QueryParameters}", queryParameters);
        var wishes = await wishesRepository.GetPagedListByQueryAsync(queryParameters, cancellationToken);

        logger.LogInformation("Retrieved wishes with meta data: {@MetaData}", wishes.MetaData);
        return wishes.Adapt<PagedListResponse<WishResponse>>();
    }

    public async Task<WishResponse?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting wish with ID: {WishId}", id);
        var wish = await wishesRepository.GetByIdAsync(id, cancellationToken) ?? throw new WishNotFoundException(id);

        var wishResponse = wish.Adapt<WishResponse>();

        logger.LogDebug("Successfully retrieved wish with ID: {WishId}", id);
        return wishResponse;
    }

    public async Task UpdateAsync(string id, WishUpdateRequest wishRequest, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating wish with ID: {WishId}", id);

        var wish = await wishesRepository.GetByIdAsync(id, cancellationToken) ?? throw new WishNotFoundException(id);

        wishRequest.Adapt(wish);

        await wishesRepository.UpdateAsync(wish, cancellationToken);
        logger.LogInformation("Successfully updated wish with ID: {WishId}", id);
    }
}
