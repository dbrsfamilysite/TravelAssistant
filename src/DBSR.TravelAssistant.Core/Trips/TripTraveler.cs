namespace DBSR.TravelAssistant.Core.Trips;

public class TripTraveler
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TripId { get; set; }

    public Guid TravelerId { get; set; }

    public string Role { get; set; } = "Member";

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}