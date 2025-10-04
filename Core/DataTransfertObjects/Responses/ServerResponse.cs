namespace DataTransfertObjects.Responses;

public record ServerResponse : IBaseDto
{
    public string Id { get; init; }
    public string HostName { get; init; }
    public string AppName { get; init; }
    public string Port { get; init; }

    public string Type { get; init; }
    public string RunMode { get; init; }
    public ServerStatusResponse LatestStatus { get; init; }

    public string CronStartTime { get; init; }
    public string CronStopTime { get; init; }

    public string Version { get; init; }
    public string Base { get; init; }
    public IList<string> BaseChain { get; init; }
    public IList<string> Tags { get; init; }

    public IList<string> Configs { get; init; }
    public IList<string> ConfigPaths { get; init; }
    public IList<string> Dictionaries { get; init; }
    public Dictionary<string, string> Dictionary { get; init; }
    public IList<string> DictionaryPaths { get; init; }
}
