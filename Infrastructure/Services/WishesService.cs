#nullable enable
using Application.Abstractions.Services;
using Application.UseCases.Wishes.GetByQuery;
using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Errors;
using Domain.Shared.Common;
using Mapster;
using DataTransfertObjects.Requests;
using DataTransfertObjects.Responses;
using Microsoft.Extensions.Logging;

namespace Services;

public sealed class WishesService(IWishesRepository wishesRepository, ILogger<WishesService> logger, IUsersRepository usersRepository) : IWishesService
{
    public async Task<PagedList<WishResponse>> GetPagedListByQueryAsync(WishQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting wishes with query parameters: {@QueryParameters}", query);
        var wishes = await wishesRepository.GetPagedListByQueryAsync(query, cancellationToken);

        var wishResponses = wishes.Adapt<List<WishResponse>>();

        logger.LogInformation("Retrieved wishes with meta data: {@MetaData}", wishes.MetaData);
        return new PagedList<WishResponse>(wishResponses, wishes.MetaData);
    }

    public async Task<WishDetailedResponse?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving wish with ID: {WishId}", id);
        Wish wish = await wishesRepository.GetByIdAsync(id, cancellationToken) ?? throw new WishNotFoundException(id);

        User? user = await usersRepository.GetByIdAsync(wish.UserId, cancellationToken);

        if (user is null) logger.LogWarning("Wish {WishId} refers to a missing user (UserId: {UserId})", id, wish.UserId);

        logger.LogInformation("Wish {WishId} retrieved.", id);

        return wish.Adapt<WishDetailedResponse>() with
        {
            User = user?.Adapt<UserResponse>()
        };
    }

    public async Task<WishResponse> CreateAsync(WishCreateRequest wishRequest, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating new wish with TITLE: {WishTitle}", wishRequest.Title);

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

        logger.LogDebug("Applying updates to wish {WishId}: Type={NewType}, Type={NewType}", id, wishRequest.Type, wishRequest.Type);
        await wishesRepository.UpdateAsync(wish, cancellationToken);

        logger.LogInformation("Successfully updated wish with ID: {WishId}", id);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting user with ID: {UserId}", id);

        var user = await usersRepository.GetByIdAsync(id, cancellationToken) ?? throw new UserNotFoundException(id);

        await usersRepository.DeleteAsync(user, cancellationToken);

        logger.LogInformation("Successfully deleted user with ID: {UserId}", id);
    }
}
