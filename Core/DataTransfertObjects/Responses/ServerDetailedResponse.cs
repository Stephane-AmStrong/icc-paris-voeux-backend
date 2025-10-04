namespace DataTransfertObjects.Responses;

public record ServerDetailedResponse : ServerResponse
{
    public IList<ServerStatusResponse> Statuses { get; init; }
}
