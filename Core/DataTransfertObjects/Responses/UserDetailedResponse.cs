namespace DataTransfertObjects.Responses;

public record UserDetailedResponse : UserResponse
{
    public IList<WishResponse> Wishes { get; init; }
}
