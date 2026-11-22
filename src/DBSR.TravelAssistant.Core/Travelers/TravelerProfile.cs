namespace DBSR.TravelAssistant.Core.Travelers;

public class TravelerProfile
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TravelerId { get; set; }

    public string? Nationality { get; set; }

    public string? PreferredLanguage { get; set; }

    public string? HomeCountry { get; set; }

    public string? HomeCity { get; set; }

    public string? TimeZone { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
