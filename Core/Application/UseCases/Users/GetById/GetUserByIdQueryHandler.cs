using Application.Abstractions.Handlers;
using Application.Abstractions.Services;
using DataTransfertObjects.Responses;

namespace Application.UseCases.Users.GetById;

public class GetUserByIdQueryHandler(IUsersService usersService) : IQueryHandler<GetUserByIdQuery, UserDetailedResponse>
{
    public Task<UserDetailedResponse> HandleAsync(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        return usersService.GetByIdAsync(query.Id, cancellationToken);
    }
}
