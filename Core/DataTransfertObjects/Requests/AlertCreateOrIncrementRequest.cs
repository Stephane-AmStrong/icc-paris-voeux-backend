#nullable enable
using DataTransfertObjects.Enumerations;

namespace DataTransfertObjects.Requests;

public record AlertCreateOrIncrementRequest
{
    public string? ServerId { get; init; }
    public AlertType? Type { get; init; }
    public string? Message { get; init; }
    public AlertSeverity? Severity { get; init; }
}
