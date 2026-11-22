namespace DBSR.TravelAssistant.Core.Transportation;

public class TransportProvider
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public string? Code { get; set; }

    public TransportMode TransportMode { get; set; }

    public string? WebsiteUrl { get; set; }

    public string? SupportPhone { get; set; }

    public string? CountryCode { get; set; }
}