using DataTransfertObjects.Enumerations;

namespace DataTransfertObjects.Responses;

public record AlertResponse : IBaseDto
{
    public string Id { get; init; }
    public string ServerId { get; init; }
    public AlertType Type { get; init; }
    public string Message { get; init; }
    public AlertSeverity Severity { get; init; }
    public int Occurrence { get; init; }
    public AlertStatus Status { get; init; }
    public DateTime OccurredAt { get; init; }
}
