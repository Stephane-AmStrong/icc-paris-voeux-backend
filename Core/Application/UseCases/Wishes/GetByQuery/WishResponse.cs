using Application.UseCases.Wishes.Create;

namespace Application.UseCases.Wishes.GetByQuery;

public record WishResponse : WishCreateRequest
{
    public string Id { get; init; }
}
