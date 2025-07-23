#nullable enable
using Application.Services.Abstractions;
using Application.UseCases.Wishes.Create;
using Application.UseCases.Wishes.GetById;
using Application.UseCases.Wishes.GetByQuery;
using Application.UseCases.Wishes.Update;
using Domain.Entities;
using Domain.Errors;
using Domain.Repositories.Abstractions;
using Domain.Shared.Common;
using Mapster;
using Microsoft.Extensions.Logging;

namespace Services;

public sealed class WishesService(
    IWishesRepository wishesRepository,
    ILogger<WishesService> logger
    ) : IWishesService
{
    public async Task<PagedList<WishResponse>> GetPagedListByQueryAsync(WishQuery queryParameters, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting wishes with query parameters: {@QueryParameters}", queryParameters);
        var wishes = await wishesRepository.GetPagedListByQueryAsync(queryParameters, cancellationToken);

        var wishResponses = wishes.Adapt<List<WishResponse>>();

        logger.LogInformation("Retrieved wishes with meta data: {@MetaData}", wishes.MetaData);
        return new PagedList<WishResponse>(wishResponses, wishes.MetaData);
    }

    public async Task<WishDetailedResponse?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting wish with ID: {WishId}", id);
        var wish = await wishesRepository.GetByIdAsync(id, cancellationToken) ?? throw new WishNotFoundException(id);

        var wishDetailedResponse = wish.Adapt<WishDetailedResponse>();

        logger.LogDebug("Successfully retrieved wish with ID: {WishId}", id);
        return wishDetailedResponse;
    }

    public async Task<WishResponse> CreateAsync(WishCreateRequest wishRequest, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating new wish with email: {WishEmail}", wishRequest.Email);
        var wish = wishRequest.Adapt<Wish>();

        await wishesRepository.CreateAsync(wish, cancellationToken);

        logger.LogInformation("Successfully created wish with ID: {WishId}", wish.Id);
        return wish.Adapt<WishResponse>();
    }

    public async Task UpdateAsync(string id, WishUpdateRequest wishRequest, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating wish with ID: {WishId}", id);

        var wish = await wishesRepository.GetByIdAsync(id, cancellationToken) ?? throw new WishNotFoundException(id);

        wishRequest.Adapt(wish);

        await wishesRepository.UpdateAsync(wish, cancellationToken);
        logger.LogInformation("Successfully updated wish with ID: {WishId}", id);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting wish with ID: {WishId}", id);
        var wish = await wishesRepository.GetByIdAsync(id, cancellationToken) ?? throw new WishNotFoundException(id);

        logger.LogInformation("Successfully deleted wish with ID: {WishId}", id);
        await wishesRepository.DeleteAsync(wish, cancellationToken);
    }
}
