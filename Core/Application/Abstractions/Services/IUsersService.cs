#nullable enable
using Application.UseCases.Users.GetByQuery;
using Domain.Shared.Common;
using DataTransfertObjects.Requests;
using DataTransfertObjects.Responses;

namespace Application.Abstractions.Services;

public interface IUsersService
{
    Task<PagedList<UserResponse>> GetPagedListByQueryAsync(UserQuery query, CancellationToken cancellationToken);
    Task<UserDetailedResponse?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<UserResponse> CreateAsync(UserCreateRequest userRequest, CancellationToken cancellationToken);
    Task UpdateAsync(string id, UserUpdateRequest userRequest, CancellationToken cancellationToken);
    Task DeleteAsync(string id, CancellationToken cancellationToken);
}
