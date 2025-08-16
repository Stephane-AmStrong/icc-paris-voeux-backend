namespace Domain.Entities;

public record Pulse : BaseEntity, IBaseEntity
{
    public override required string Id { get; set; }
    public required string UserId { get; init; }
    public required string Status { get; init; }
    public DateTime RecordedAt { get; init; }

    public int Streak { get; init; }
}
