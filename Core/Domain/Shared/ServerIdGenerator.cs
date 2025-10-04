namespace Domain.Shared;

public static class ServerIdGenerator
{
    public static string Generate(string hostName, string appName) => $"{hostName};{appName}";
}
