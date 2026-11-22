namespace DBSR.TravelAssistant.Core.Itineraries;

public class Activity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ItineraryDayId { get; set; }

    public Guid? PlaceId { get; set; }

    public string Name { get; set; } = string.Empty;

    public ActivityCategory Category { get; set; } = ActivityCategory.Other;

    public string? Description { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public decimal? EstimatedCost { get; set; }

    public string? CurrencyCode { get; set; }

    public int SequenceNumber { get; set; }
}
