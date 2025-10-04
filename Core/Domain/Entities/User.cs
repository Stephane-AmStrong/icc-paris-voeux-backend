namespace Domain.Entities;

public record User : BaseEntity
{
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
}
