using DataTransfertObjects.Enumerations;

namespace DataTransfertObjects.Responses;

public record ServerStatusResponse : IBaseDto
{
    public string Id { get; init; }
    public string ServerId { get; init; }
    public ServerStatus Status { get; init; }
    public DateTime RecordedAt { get; init; }
}
