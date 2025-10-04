namespace DataTransfertObjects.Requests;

public record ServerCreateRequest
{
    public string HostName { get; init; }
    public string Port { get; init; }
    public string AppName { get; init; }
    public string Type { get; init; }
    public string Version { get; init; }
}
