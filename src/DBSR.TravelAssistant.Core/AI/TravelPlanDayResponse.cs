namespace DBSR.TravelAssistant.Core.AI;

public class TravelPlanDayResponse
{
    public int DayNumber { get; set; }

    public DateOnly Date { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public List<TravelPlanActivityResponse> Activities { get; set; } = [];
}