#nullable enable
using Domain.Entities;
using Domain.Shared.Common;

namespace Application.UseCases.Wishes.GetByQuery;

public record WishQuery : BaseQueryParameters<Wish>
{

    public string? WithEmail { get; set; }
    public string? WithSpiritually { get; set; }

    public WishQuery(string? withEmail, string? withSpiritually, string? searchTerm, string? orderBy, int? page, int? pageSize)
    {
        WithEmail = withEmail;
        WithSpiritually = withSpiritually;
        SearchTerm = searchTerm;
        OrderBy = orderBy;
        Page = page ?? 1;
        PageSize = pageSize;
        
        if (WithEmail != null || WithSpiritually != null)
        {
            SetFilterExpression(wish => (WithEmail == null || wish.Email == WithEmail) && (WithSpiritually == null || wish.Spiritually == WithSpiritually));
        }
    }
}
