using DataTransfertObjects.Requests;

namespace DataTransfertObjects.Responses;

public record WishResponse : WishCreateRequest, IBaseDto
{
    public required string Id { get; init; }
}
