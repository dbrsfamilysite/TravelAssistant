
namespace DBSR.TravelAssistant.Core.TravelInsurance;

public class TravelInsurancePolicy
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TripId { get; set; }

    public Guid TravelInsuranceProviderId { get; set; }

    public string PolicyNumber { get; set; } = string.Empty;

    public string PolicyName { get; set; } = string.Empty;

    public TravelInsuranceStatus Status { get; set; } = TravelInsuranceStatus.Draft;

    public DateOnly CoverageStartDate { get; set; }

    public DateOnly CoverageEndDate { get; set; }

    public decimal PremiumAmount { get; set; }

    public string CurrencyCode { get; set; } = "USD";

    public decimal? TotalCoverageLimit { get; set; }

    public string? PolicyDocumentUrl { get; set; }

    public string? TermsAndConditionsUrl { get; set; }

    public string? EmergencyContactPhone { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
