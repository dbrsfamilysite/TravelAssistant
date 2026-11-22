namespace DBSR.TravelAssistant.Core.Itineraries;

public class Itinerary
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TripId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsAiGenerated { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
