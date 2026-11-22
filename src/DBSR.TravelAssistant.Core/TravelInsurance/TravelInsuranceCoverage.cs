
namespace DBSR.TravelAssistant.Core.TravelInsurance;

public class TravelInsuranceCoverage
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TravelInsurancePolicyId { get; set; }

    public TravelInsuranceCoverageType CoverageType { get; set; }

    public string? Description { get; set; }

    public decimal CoverageLimit { get; set; }

    public decimal? Deductible { get; set; }

    public string CurrencyCode { get; set; } = "USD";

    public bool IsIncluded { get; set; } = true;

    public string? Notes { get; set; }
}
