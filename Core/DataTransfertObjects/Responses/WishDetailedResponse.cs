namespace DataTransfertObjects.Responses;

public record WishDetailedResponse : WishResponse
{
    public UserResponse User { get; init; }
}
