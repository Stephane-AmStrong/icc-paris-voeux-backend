namespace WebApi.Client.DataTransferObjects;

public record WishCreateRequest
{
    public string Email { get; init; }
    public string Spiritually { get; init; }
    public string FamiliallyRelationally { get; init; }
    public string FinanciallyMaterially { get; init; }
    public string ProfessionallyAcademically { get; init; }
    public string Other { get; init; }
}
