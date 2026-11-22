
namespace DBSR.TravelAssistant.Core.TravelInsurance;

public class TravelInsuranceBeneficiary
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TravelInsurancePolicyId { get; set; }

    public Guid TravelerId { get; set; }

    public bool IsPrimaryInsured { get; set; }

    public string? RelationshipToPolicyHolder { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
