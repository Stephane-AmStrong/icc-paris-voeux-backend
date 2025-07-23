using Application.DataTransfertObjects;

namespace Application.UseCases.Wishes.GetById;

public record WishDetailedResponse : WishResponse
{
    public string Id { get; init; }
}
