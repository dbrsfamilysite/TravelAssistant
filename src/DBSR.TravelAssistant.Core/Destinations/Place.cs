namespace DBSR.TravelAssistant.Core.Destinations;

public class Place
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid DestinationId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Category { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public decimal? EstimatedEntryCost { get; set; }

    public string? WebsiteUrl { get; set; }
}
