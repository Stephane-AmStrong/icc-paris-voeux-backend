namespace WebApi.Client.DataTransferObjects;

public record WishResponse : WishCreateRequest, IBaseDto
{
    public string Id { get; set; }
}
