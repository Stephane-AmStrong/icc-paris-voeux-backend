using DataTransfertObjects.Requests;

namespace DataTransfertObjects.Responses;

public record UserResponse : UserCreateRequest, IBaseDto
{
    public required string Id { get; init; }
}
