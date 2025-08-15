#nullable enable
using Application.Abstractions.Handlers;
using Application.Abstractions.Services;
using Domain.Shared.Common;
using DataTransfertObjects.Responses;

namespace Application.UseCases.Users.GetByQuery;

public class GetUserQueryHandler(IUsersService usersService) : IQueryHandler<GetUserQuery, PagedList<UserResponse>>
{
    public Task<PagedList<UserResponse>> HandleAsync(GetUserQuery query, CancellationToken cancellationToken)
    {
        return usersService.GetPagedListByQueryAsync(new UserQuery(query.Parameters), cancellationToken);
    }
}
