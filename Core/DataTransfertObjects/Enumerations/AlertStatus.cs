using System.Text.Json.Serialization;

namespace DataTransfertObjects.Enumerations;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AlertStatus
{
    Active,
    Muted,
    Resolved,
    Assigned
}
