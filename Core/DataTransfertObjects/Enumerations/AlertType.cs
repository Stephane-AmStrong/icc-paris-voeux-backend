using System.Text.Json.Serialization;

namespace DataTransfertObjects.Enumerations;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AlertType
{
    ApiHealthCheck,
    CPU,
    Disk,
    Latency,
    MarketConnection,
    MemoProvider,
    Memory,
    PriceDiff,
    Recorder,
    Scripts
}
