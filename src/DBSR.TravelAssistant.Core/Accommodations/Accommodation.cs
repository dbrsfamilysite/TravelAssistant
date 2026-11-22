namespace DBSR.TravelAssistant.Core.Accommodations;

public class Accommodation
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid DestinationId { get; set; }

    public string Name { get; set; } = string.Empty;

    public AccommodationType Type { get; set; }

    public string? Description { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? CountryCode { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public string? TimeZoneId { get; set; }

    public string? WebsiteUrl { get; set; }

    public string? ContactPhone { get; set; }

    public decimal? Rating { get; set; }

    public string? CurrencyCode { get; set; }
}