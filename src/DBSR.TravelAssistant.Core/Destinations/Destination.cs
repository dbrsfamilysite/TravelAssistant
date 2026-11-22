namespace DBSR.TravelAssistant.Core.Destinations;

public class Destination
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid CountryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DestinationType DestinationType { get; set; } = DestinationType.Other;

    public string? BestTimeToVisit { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
