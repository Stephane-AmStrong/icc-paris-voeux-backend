namespace Domain.Entities;

public record User : BaseEntity, IBaseEntity
{
    public override required string Id { get; set; }
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
}
