using DBSR.TravelAssistant.Core.Itineraries;

namespace DBSR.TravelAssistant.Core.AI;

public class TravelPlanActivityResponse
{
    public string Name { get; set; } = string.Empty;

    public ActivityCategory Category { get; set; } = ActivityCategory.Other;

    public string? Description { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public decimal? EstimatedCost { get; set; }

    public string? CurrencyCode { get; set; }

    public int SequenceNumber { get; set; }
}