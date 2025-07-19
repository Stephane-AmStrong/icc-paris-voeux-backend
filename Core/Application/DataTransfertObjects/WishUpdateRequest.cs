using System.Text.Json.Serialization;

namespace Application.DataTransfertObjects;

public record WishUpdateRequest
{
    public string Spiritually { get; init; }
    public string FamiliallyRelationally { get; init; }
    public string FinanciallyMaterially { get; init; }
    public string ProfessionallyAcademically { get; init; }
    public string Other { get; init; }
    public string Email { get; init; }
}
