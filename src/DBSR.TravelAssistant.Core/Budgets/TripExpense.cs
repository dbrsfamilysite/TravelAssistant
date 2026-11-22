namespace DBSR.TravelAssistant.Core.Budgets;

public class TripExpense
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TripBudgetId { get; set; }

    public BudgetCategory Category { get; set; }

    public string Description { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string CurrencyCode { get; set; } = "USD";

    public DateOnly ExpenseDate { get; set; }

    public bool IsEstimated { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
