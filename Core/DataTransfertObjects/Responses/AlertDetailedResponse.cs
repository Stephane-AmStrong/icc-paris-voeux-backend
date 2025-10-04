namespace DataTransfertObjects.Responses;

public record AlertDetailedResponse : AlertResponse
{
    public ServerResponse Server { get; init; }
}
