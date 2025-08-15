#nullable enable

namespace Domain.Entities;

public record Wish : IBaseEntity
{
    public required string Id { get; init; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? FulfilledAt { get; set; }
    public required string UserId { get; init; }
}