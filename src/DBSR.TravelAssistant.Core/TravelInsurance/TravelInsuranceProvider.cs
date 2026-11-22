
namespace DBSR.TravelAssistant.Core.TravelInsurance;

public class TravelInsuranceProvider
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public string? Code { get; set; }

    public string? WebsiteUrl { get; set; }

    public string? SupportPhone { get; set; }

    public string? SupportEmail { get; set; }

    public string? CountryCode { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
