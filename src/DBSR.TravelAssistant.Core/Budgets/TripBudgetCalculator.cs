
using DBSR.TravelAssistant.Core.Trips;

namespace DBSR.TravelAssistant.Core.Budgets;

public sealed class TripBudgetCalculator
{
    public TripBudgetSummary Calculate(
        Trip trip,
        TripBudget budget,
        IEnumerable<TripExpense> expenses,
        IEnumerable<CurrencyConversion> conversions)
    {
        ArgumentNullException.ThrowIfNull(trip);
        ArgumentNullException.ThrowIfNull(budget);
        ArgumentNullException.ThrowIfNull(expenses);
        ArgumentNullException.ThrowIfNull(conversions);

        if (budget.TotalEstimatedBudget <= 0)
            throw new ArgumentException("The budget limit must be greater than zero.");

        if (budget.TripId != trip.Id)
            throw new ArgumentException("The budget does not belong to the supplied trip.");

        if (trip.StartDate.HasValue &&
            trip.EndDate.HasValue &&
            trip.StartDate > trip.EndDate)
        {
            throw new ArgumentException("The trip start date cannot be after its end date.");
        }

        var budgetCurrency = NormalizeCurrency(budget.CurrencyCode);
        var expenseList = expenses.ToList();
        var conversionList = conversions.ToList();

        decimal estimatedTotal = 0;
        decimal actualTotal = 0;

        var dailyTotals = new Dictionary<DateOnly, (decimal Estimated, decimal Actual)>();

        var categoryTotals = new Dictionary<BudgetCategory, (decimal Estimated, decimal Actual)>();

        foreach (var expense in expenseList)
        {
            if (expense.TripBudgetId != budget.Id)
                throw new ArgumentException(
                    $"Expense {expense.Id} does not belong to the supplied budget.");

            if (expense.Amount < 0)
                throw new ArgumentException(
                    $"Expense {expense.Id} has a negative amount.");

            if (trip.StartDate.HasValue && expense.ExpenseDate < trip.StartDate.Value)
                throw new ArgumentException(
                    $"Expense {expense.Id} occurs before the trip starts.");

            if (trip.EndDate.HasValue && expense.ExpenseDate > trip.EndDate.Value)
                throw new ArgumentException(
                    $"Expense {expense.Id} occurs after the trip ends.");

            if (!Enum.IsDefined(expense.Category))
                throw new ArgumentException(
                    $"Expense {expense.Id} has an invalid category.");

            var expenseCurrency = NormalizeCurrency(expense.CurrencyCode);

            var convertedAmount = ConvertAmount(
                expense.Amount,
                expenseCurrency,
                budgetCurrency,
                expense.ExpenseDate,
                conversionList);

            if (expense.IsEstimated)
                estimatedTotal += convertedAmount;
            else
                actualTotal += convertedAmount;

            dailyTotals.TryGetValue(
                expense.ExpenseDate,
                out var daily);

            dailyTotals[expense.ExpenseDate] = expense.IsEstimated
                ? (daily.Estimated + convertedAmount, daily.Actual)
                : (daily.Estimated, daily.Actual + convertedAmount);

            categoryTotals.TryGetValue(
                expense.Category,
                out var category);

            categoryTotals[expense.Category] = expense.IsEstimated
                ? (category.Estimated + convertedAmount, category.Actual)
                : (category.Estimated, category.Actual + convertedAmount);
        }

        var totalExpenses = estimatedTotal + actualTotal;

        return new TripBudgetSummary
        {
            TripId = trip.Id,
            BudgetLimit = budget.TotalEstimatedBudget,
            CurrencyCode = budgetCurrency,

            TotalEstimatedExpenses = estimatedTotal,
            TotalActualExpenses = actualTotal,
            TotalExpenses = totalExpenses,

            RemainingBudget = Math.Max(
                0,
                budget.TotalEstimatedBudget - totalExpenses),

            AmountOverBudget = Math.Max(
                0,
                totalExpenses - budget.TotalEstimatedBudget),

            IsOverBudget = totalExpenses > budget.TotalEstimatedBudget,

            EstimatedExpensePercentage = Math.Round(
                estimatedTotal / budget.TotalEstimatedBudget * 100,
                2),

            ActualExpensePercentage = Math.Round(
                actualTotal / budget.TotalEstimatedBudget * 100,
                2),

            ExpenseCount = expenseList.Count,

            DailyBreakdown = dailyTotals
                .OrderBy(x => x.Key)
                .Select(x => new DailyBudgetSummary
                {
                    Date = x.Key,
                    EstimatedExpenses = x.Value.Estimated,
                    ActualExpenses = x.Value.Actual,
                    TotalExpenses = x.Value.Estimated + x.Value.Actual
                })
                .ToList(),

            CategoryBreakdown = categoryTotals
                .OrderBy(x => x.Key)
                .Select(x => new CategoryBudgetSummary
                {
                    Category = x.Key,
                    EstimatedExpenses = x.Value.Estimated,
                    ActualExpenses = x.Value.Actual,
                    TotalExpenses = x.Value.Estimated + x.Value.Actual
                })
                .ToList()
        };
    }

    private static decimal ConvertAmount(
        decimal amount,
        string fromCurrency,
        string toCurrency,
        DateOnly expenseDate,
        IReadOnlyCollection<CurrencyConversion> conversions)
    {
        if (fromCurrency == toCurrency)
            return amount;

        var applicableDate = expenseDate;

        var directRate = FindLatestRate(
            conversions,
            fromCurrency,
            toCurrency,
            applicableDate);

        if (directRate is not null)
            return amount * directRate.ExchangeRate;

        var inverseRate = FindLatestRate(
            conversions,
            toCurrency,
            fromCurrency,
            applicableDate);

        if (inverseRate is not null)
            return amount / inverseRate.ExchangeRate;

        throw new InvalidOperationException(
            $"No currency conversion rate is available from {fromCurrency} " +
            $"to {toCurrency} on or before {expenseDate:yyyy-MM-dd}.");
    }

    private static CurrencyConversion? FindLatestRate(
        IEnumerable<CurrencyConversion> conversions,
        string fromCurrency,
        string toCurrency,
        DateOnly expenseDate)
    {
        var matchingRates = conversions
            .Where(x =>
                string.Equals(x.FromCurrencyCode, fromCurrency,
                    StringComparison.OrdinalIgnoreCase) &&
                string.Equals(x.ToCurrencyCode, toCurrency,
                    StringComparison.OrdinalIgnoreCase) &&
                DateOnly.FromDateTime(x.RateDate) <= expenseDate)
            .OrderByDescending(x => x.RateDate)
            .ToList();

        if (matchingRates.Count == 0)
            return null;

        var latestRateDate = matchingRates[0].RateDate;

        var latestRates = matchingRates
            .Where(x => x.RateDate == latestRateDate)
            .ToList();

        if (latestRates.Count > 1)
            throw new InvalidOperationException(
                $"Multiple conversion rates exist for {fromCurrency} to " +
                $"{toCurrency} on {latestRateDate:yyyy-MM-dd HH:mm:ss}.");

        if (latestRates[0].ExchangeRate <= 0)
            throw new InvalidOperationException(
                $"The exchange rate from {fromCurrency} to {toCurrency} must be positive.");

        return latestRates[0];
    }

    private static string NormalizeCurrency(string currencyCode)
    {
        if (string.IsNullOrWhiteSpace(currencyCode))
            throw new ArgumentException("Currency code cannot be empty.");

        var normalized = currencyCode.Trim().ToUpperInvariant();

        if (normalized.Length != 3 ||
            normalized.Any(c => c < 'A' || c > 'Z'))
        {
            throw new ArgumentException(
                $"Invalid currency code: {currencyCode}. Expected a three-letter code.");
        }

        return normalized;
    }
}
