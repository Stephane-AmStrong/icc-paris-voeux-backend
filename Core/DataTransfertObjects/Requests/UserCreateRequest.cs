namespace DataTransfertObjects.Requests;

public record UserCreateRequest
{
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
}
