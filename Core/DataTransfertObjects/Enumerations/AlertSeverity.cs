using System.Text.Json.Serialization;

namespace DataTransfertObjects.Enumerations;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AlertSeverity
{
    Information,
    Warning,
    Error
}
