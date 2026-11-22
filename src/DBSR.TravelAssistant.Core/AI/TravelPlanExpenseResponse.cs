using DBSR.TravelAssistant.Core.Budgets;

namespace DBSR.TravelAssistant.Core.AI;

public class TravelPlanExpenseResponse
{
    public BudgetCategory Category { get; set; }

    public string Description { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string CurrencyCode { get; set; } = "USD";

    public DateOnly ExpenseDate { get; set; }

    public bool IsEstimated { get; set; } = true;
}