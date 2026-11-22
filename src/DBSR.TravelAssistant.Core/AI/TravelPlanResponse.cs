namespace DBSR.TravelAssistant.Core.AI;

public class TravelPlanResponse
{
    public string TripName { get; set; } = string.Empty;

    public string Overview { get; set; } = string.Empty;

    public string CurrencyCode { get; set; } = "USD";

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public int NumberOfTravelers { get; set; }

    public decimal? BudgetLimit { get; set; }

    public List<string> Assumptions { get; set; } = [];

    public List<TravelPlanDayResponse> Days { get; set; } = [];

    public List<TravelPlanExpenseResponse> Expenses { get; set; } = [];
}