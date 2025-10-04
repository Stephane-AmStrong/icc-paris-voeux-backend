namespace Domain.Shared;

public static class ConnectionIdGenerator
{
    public static string Generate(string serverId, string clientId) => $"{serverId};{clientId}";
}
