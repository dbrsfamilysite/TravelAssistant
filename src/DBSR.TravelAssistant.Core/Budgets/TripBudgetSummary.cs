namespace DBSR.TravelAssistant.Core.Budgets;

public class TripBudgetSummary
{
    public Guid TripId { get; set; }

    public decimal BudgetLimit { get; set; }

    public string CurrencyCode { get; set; } = "USD";

    public decimal TotalEstimatedExpenses { get; set; }

    public decimal TotalActualExpenses { get; set; }

    public decimal TotalExpenses { get; set; }

    public decimal RemainingBudget { get; set; }

    public decimal AmountOverBudget { get; set; }

    public bool IsOverBudget { get; set; }

    public decimal EstimatedExpensePercentage { get; set; }

    public decimal ActualExpensePercentage { get; set; }

    public int ExpenseCount { get; set; }

    public List<DailyBudgetSummary> DailyBreakdown { get; set; } = [];

    public List<CategoryBudgetSummary> CategoryBreakdown { get; set; } = [];
}

public class DailyBudgetSummary
{
    public DateOnly Date { get; set; }

    public decimal EstimatedExpenses { get; set; }

    public decimal ActualExpenses { get; set; }

    public decimal TotalExpenses { get; set; }
}

public class CategoryBudgetSummary
{
    public BudgetCategory Category { get; set; }

    public decimal EstimatedExpenses { get; set; }

    public decimal ActualExpenses { get; set; }

    public decimal TotalExpenses { get; set; }
}