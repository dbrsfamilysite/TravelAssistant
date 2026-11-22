namespace DBSR.TravelAssistant.Core.Trips;

public class Trip
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid OwnerTravelerId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public TripPurpose Purpose { get; set; } = TripPurpose.Leisure;

    public TripStatus Status { get; set; } = TripStatus.Draft;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
