namespace DBSR.TravelAssistant.Core.Itineraries;

public class ItineraryDay
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ItineraryId { get; set; }

    public int DayNumber { get; set; }

    public DateOnly Date { get; set; }

    public string? Title { get; set; }

    public string? Notes { get; set; }
}
