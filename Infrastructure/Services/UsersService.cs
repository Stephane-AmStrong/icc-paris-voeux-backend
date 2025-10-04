#nullable enable
using Application.Abstractions.Services;
using Application.UseCases.Users.GetByQuery;
using DataTransfertObjects.Requests;
using DataTransfertObjects.Responses;
using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Errors;
using Domain.Shared.Common;
using Mapster;
using Microsoft.Extensions.Logging;

namespace Services;

public sealed class UsersService(IUsersRepository usersRepository, IWishesRepository wishesRepository, ILogger<UsersService> logger) : IUsersService
{
    public async Task<PagedList<UserResponse>> GetPagedListByQueryAsync(UserQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting users with query parameters: {@QueryParameters}", query);
        var users = await usersRepository.GetPagedListByQueryAsync(query, cancellationToken);

        var userResponses = users.Adapt<List<UserResponse>>();

        logger.LogInformation("Retrieved users with meta data: {@MetaData}", users.MetaData);
        return new PagedList<UserResponse>(userResponses, users.MetaData);
    }

    public async Task<UserDetailedResponse?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving detailed user information for ID: {UserId}", id);

        User user = await usersRepository.GetByIdAsync(id, cancellationToken) ?? throw new UserNotFoundException(id);

        logger.LogDebug("User {UserId} found: {UserLastName} {UserFirstName}", id, user.LastName, user.FirstName);

        var wishes = await wishesRepository.FindByConditionAsync(wish => wish.UserId == id, cancellationToken);

        logger.LogDebug("Retrieved associated data for user {UserId}: {WishCount} wishs, {WishCount} wishes", id, wishes.Count, wishes.Count);

        var userResponse = user.Adapt<UserDetailedResponse>();

        logger.LogInformation("Successfully retrieved detailed user {UserId} with {WishCount} wishes and {WishCount} recent wishs", id, wishes.Count, wishes.Count);

        return userResponse with
        {
            Wishes = wishes.Adapt<IList<WishResponse>>()
        };
    }

    public async Task<UserResponse> CreateAsync(UserCreateRequest userRequest, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating new user with EMAIL: {UserEmail}", userRequest.Email);

        var user = userRequest.Adapt<User>();

        await usersRepository.CreateAsync(user, cancellationToken);

        logger.LogInformation("Successfully created user with ID: {UserId}", user.Id);

        return user.Adapt<UserResponse>();
    }

    public async Task UpdateAsync(string id, UserUpdateRequest userRequest, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating user with ID: {UserId}", id);
        var user = await usersRepository.GetByIdAsync(id, cancellationToken) ?? throw new UserNotFoundException(id);

        userRequest.Adapt(user);

        logger.LogDebug("Applying updates to user {UserId}: LastName={NewLastName}, FirstName={NewFirstName}", id, userRequest.LastName, userRequest.FirstName);
        await usersRepository.UpdateAsync(user, cancellationToken);

        logger.LogInformation("Successfully updated user with ID: {UserId}", id);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting user with ID: {UserId}", id);

        var user = await usersRepository.GetByIdAsync(id, cancellationToken) ?? throw new UserNotFoundException(id);

        await usersRepository.DeleteAsync(user, cancellationToken);

        logger.LogInformation("Successfully deleted user with ID: {UserId}", id);
    }
}
