namespace DBSR.TravelAssistant.Core.Accommodations;

public class AccommodationGuest
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid AccommodationBookingId { get; set; }

    public Guid TravelerId { get; set; }

    public bool IsPrimaryGuest { get; set; }

    public string? GuestNotes { get; set; }
}