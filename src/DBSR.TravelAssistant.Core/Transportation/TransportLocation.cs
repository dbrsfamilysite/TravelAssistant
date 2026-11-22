namespace DBSR.TravelAssistant.Core.Transportation;

public class TransportLocation
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public string? Code { get; set; }

    public TransportLocationType? LocationType { get; set; }

    public string? City { get; set; }

    public string? CountryCode { get; set; }

    public string? Address { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public string? TimeZoneId { get; set; }
}