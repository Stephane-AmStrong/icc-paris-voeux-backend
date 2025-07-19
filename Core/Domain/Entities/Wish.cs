using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Wish
{
    public string Id { get; init; }
    public string Spiritually { get; init; }
    public string FamiliallyRelationally { get; init; }
    public string FinanciallyMaterially { get; init; }
    public string ProfessionallyAcademically { get; init; }
    public string Other { get; init; }
    public string Email { get; init; }
};

