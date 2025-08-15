#nullable enable
using DataTransfertObjects.Enumerations;

namespace DataTransfertObjects.Requests;

public record WishCreateRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public WishType? Type { get; init; }
    public string? UserId { get; init; }
}
