#nullable enable
using Domain.Entities;
using Domain.Shared.Common;
using DataTransfertObjects.QueryParameters;

namespace Application.UseCases.Wishes.GetByQuery;

public record WishQuery : BaseQuery<Wish>
{
    public WishQuery(WishQueryParameters queryParameters) : base(queryParameters.SearchTerm, queryParameters.OrderBy, queryParameters.Page, queryParameters.PageSize)
    {
        if (!string.IsNullOrWhiteSpace(queryParameters.WithUserId) || (!string.IsNullOrWhiteSpace(queryParameters.WithTitle) || queryParameters.OfType is not null || queryParameters.CreatedBefore is not null || queryParameters.CreatedAfter is not null || queryParameters.FulfilledBefore is not null || queryParameters.FulfilledAfter is not null))
        {
            SetFilterExpression
            (
                wish => (string.IsNullOrWhiteSpace(queryParameters.WithUserId) || wish.UserId == queryParameters.WithUserId) &&
                        (string.IsNullOrWhiteSpace(queryParameters.WithTitle) || wish.Title == queryParameters.WithTitle) &&
                        (queryParameters.OfType == null || wish.Type == queryParameters.OfType.ToString()) &&
                        (queryParameters.CreatedBefore == null || wish.CreatedAt < queryParameters.CreatedBefore) &&
                        (queryParameters.CreatedAfter == null || wish.CreatedAt >= queryParameters.CreatedAfter) &&
                        (queryParameters.FulfilledBefore == null || wish.FulfilledAt < queryParameters.FulfilledBefore) &&
                        (queryParameters.FulfilledAfter == null || wish.FulfilledAt >= queryParameters.FulfilledAfter)
            );
        }
    }
}
