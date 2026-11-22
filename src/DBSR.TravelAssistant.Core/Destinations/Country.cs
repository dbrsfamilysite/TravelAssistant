namespace DBSR.TravelAssistant.Core.Destinations;

public class Country
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public string CountryCode { get; set; } = string.Empty;

    public string? CurrencyCode { get; set; }

    public string? OfficialLanguage { get; set; }

    public string? DefaultTimeZoneId { get; set; }
}