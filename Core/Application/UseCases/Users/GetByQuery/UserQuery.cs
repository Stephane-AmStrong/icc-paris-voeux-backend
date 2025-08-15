#nullable enable

using Domain.Entities;
using Domain.Shared.Common;
using DataTransfertObjects.QueryParameters;

namespace Application.UseCases.Users.GetByQuery;

public record UserQuery : BaseQuery<User>
{
    public UserQuery(UserQueryParameters queryParameters) : base(queryParameters.SearchTerm, queryParameters.OrderBy, queryParameters.Page, queryParameters.PageSize)
    {
        if (!string.IsNullOrWhiteSpace(queryParameters.WithEmail) || !string.IsNullOrWhiteSpace(queryParameters.WithLastName) || !string.IsNullOrWhiteSpace(queryParameters.WithFirstName))
        {
            SetFilterExpression
            (
                user => (string.IsNullOrWhiteSpace(queryParameters.WithEmail) || user.Email == queryParameters.WithEmail) && (string.IsNullOrWhiteSpace(queryParameters.WithLastName) || user.LastName == queryParameters.WithLastName) && (string.IsNullOrWhiteSpace(queryParameters.WithFirstName) || user.FirstName == queryParameters.WithFirstName)
            );
        }
    }
}