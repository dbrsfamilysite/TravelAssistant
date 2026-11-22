namespace DBSR.TravelAssistant.Core.Trips;

public class TripDestination
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TripId { get; set; }

    public Guid DestinationId { get; set; }

    public int SequenceNumber { get; set; }

    public DateOnly? ArrivalDate { get; set; }

    public DateOnly? DepartureDate { get; set; }

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
