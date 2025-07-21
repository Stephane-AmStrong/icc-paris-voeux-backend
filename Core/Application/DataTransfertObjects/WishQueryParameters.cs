using Domain.Entities;
using Domain.Shared.Common;

namespace Application.DataTransfertObjects;

public record class WishQueryParameters : QueryParameters<Wish>
{
    public WishQueryParameters(string? withEmail, string? withSpiritually, string? searchTerm, string? orderBy, int? page, int? pageSize)
    {
        WithEmail = withEmail;
        WithSpiritually = withSpiritually;
        SearchTerm = searchTerm;
        OrderBy = orderBy;
        Page = page;
        PageSize = pageSize;
        
        if (WithEmail != null || WithSpiritually != null)
        {
            SetFilterExpression(wish =>
                (WithEmail == null || wish.Email == WithEmail) &&
                (WithSpiritually == null || wish.Spiritually == WithSpiritually));
        }
    }

    public string? WithEmail { get; set; } = null;
    public string? WithSpiritually { get; set; } = null;
}