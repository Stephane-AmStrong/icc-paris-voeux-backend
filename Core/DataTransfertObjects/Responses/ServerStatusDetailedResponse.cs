namespace DataTransfertObjects.Responses;

public record ServerStatusDetailedResponse : ServerStatusResponse
{
    public ServerResponse Server { get; init; }
}
