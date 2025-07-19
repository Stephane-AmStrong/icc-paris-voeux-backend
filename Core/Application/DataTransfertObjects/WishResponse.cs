namespace Application.DataTransfertObjects;

public record WishResponse : WishCreateRequest
{
    public string Id { get; init; }
}
