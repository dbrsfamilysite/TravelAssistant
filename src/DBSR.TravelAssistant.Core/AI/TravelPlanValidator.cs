
using DBSR.TravelAssistant.Core.Budgets;
using DBSR.TravelAssistant.Core.Itineraries;

namespace DBSR.TravelAssistant.Core.AI;

public sealed class TravelPlanValidator
{
    public List<string> Validate(TravelPlanResponse plan)
    {
        ArgumentNullException.ThrowIfNull(plan);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(plan.TripName))
            errors.Add("Trip name is required.");

        if (string.IsNullOrWhiteSpace(plan.Overview))
            errors.Add("Trip overview is required.");

        if (plan.NumberOfTravelers <= 0)
            errors.Add("Number of travelers must be greater than zero.");

        if (plan.StartDate.HasValue &&
            plan.EndDate.HasValue &&
            plan.StartDate > plan.EndDate)
        {
            errors.Add("Trip start date cannot be after end date.");
        }

        ValidateCurrency(plan.CurrencyCode, "Trip currency", errors);

        if (plan.BudgetLimit.HasValue && plan.BudgetLimit.Value <= 0)
            errors.Add("Budget limit must be greater than zero.");

        if (plan.Days is null || plan.Days.Count == 0)
            errors.Add("At least one itinerary day is required.");
        else
            ValidateDays(plan.Days, plan.StartDate, plan.EndDate, errors);

        if (plan.Expenses is not null)
            ValidateExpenses(plan.Expenses, plan.StartDate, plan.EndDate, errors);

        return errors;
    }

    private static void ValidateDays(
        List<TravelPlanDayResponse> days,
        DateOnly? startDate,
        DateOnly? endDate,
        List<string> errors)
    {
        var dayNumbers = new HashSet<int>();
        var dates = new HashSet<DateOnly>();

        foreach (var day in days)
        {
            if (day.DayNumber <= 0)
                errors.Add("Day number must be greater than zero.");

            if (!dayNumbers.Add(day.DayNumber))
                errors.Add($"Duplicate itinerary day number: {day.DayNumber}.");

            if (!dates.Add(day.Date))
                errors.Add($"Duplicate itinerary date: {day.Date}.");

            if (startDate.HasValue && day.Date < startDate.Value)
                errors.Add($"Itinerary day {day.DayNumber} occurs before the trip starts.");

            if (endDate.HasValue && day.Date > endDate.Value)
                errors.Add($"Itinerary day {day.DayNumber} occurs after the trip ends.");

            if (string.IsNullOrWhiteSpace(day.Title))
                errors.Add($"Day {day.DayNumber} must have a title.");

            if (day.Activities is null)
                continue;

            ValidateActivities(day, errors);
        }
    }

    private static void ValidateActivities(
        TravelPlanDayResponse day,
        List<string> errors)
    {
        var activities = day.Activities;

        var sequenceNumbers = new HashSet<int>();

        foreach (var activity in activities)
        {
            if (activity.SequenceNumber <= 0)
                errors.Add(
                    $"Activity '{activity.Name}' on day {day.DayNumber} must have a positive sequence number.");

            if (!sequenceNumbers.Add(activity.SequenceNumber))
                errors.Add(
                    $"Day {day.DayNumber} contains duplicate activity sequence number {activity.SequenceNumber}.");

            if (string.IsNullOrWhiteSpace(activity.Name))
                errors.Add(
                    $"Day {day.DayNumber} contains an activity without a name.");

            if (!Enum.IsDefined(activity.Category))
                errors.Add(
                    $"Day {day.DayNumber} contains an invalid activity category.");

            if (activity.StartTime.HasValue &&
                activity.EndTime.HasValue &&
                activity.EndTime.Value <= activity.StartTime.Value)
            {
                errors.Add(
                    $"Activity '{activity.Name}' ends before it starts.");
            }

            if (activity.EstimatedCost.HasValue &&
                activity.EstimatedCost.Value < 0)
            {
                errors.Add(
                    $"Activity '{activity.Name}' has a negative estimated cost.");
            }

            if (activity.CurrencyCode is not null)
            {
                ValidateCurrency(
                    activity.CurrencyCode,
                    $"Activity '{activity.Name}' currency",
                    errors);
            }
        }

        var scheduledActivities = activities
            .Where(activity => activity.StartTime.HasValue)
            .OrderBy(activity => activity.StartTime!.Value)
            .ToList();

        for (var i = 1; i < scheduledActivities.Count; i++)
        {
            var previous = scheduledActivities[i - 1];
            var current = scheduledActivities[i];

            if (previous.EndTime.HasValue &&
                current.StartTime!.Value < previous.EndTime.Value)
            {
                errors.Add(
                    $"Activities '{previous.Name}' and '{current.Name}' overlap on day {day.DayNumber}.");
            }
        }

        var sequencedActivities = activities
            .Where(activity => activity.StartTime.HasValue)
            .OrderBy(activity => activity.SequenceNumber)
            .ToList();

        for (var i = 1; i < sequencedActivities.Count; i++)
        {
            if (sequencedActivities[i].StartTime!.Value <
                sequencedActivities[i - 1].StartTime!.Value)
            {
                errors.Add(
                    $"Activity sequence does not follow chronological order on day {day.DayNumber}.");
                break;
            }
        }
    }

    private static void ValidateExpenses(
        List<TravelPlanExpenseResponse> expenses,
        DateOnly? startDate,
        DateOnly? endDate,
        List<string> errors)
    {
        foreach (var expense in expenses)
        {
            if (!Enum.IsDefined(expense.Category))
                errors.Add(
                    $"Expense '{expense.Description}' has an invalid category.");

            if (string.IsNullOrWhiteSpace(expense.Description))
                errors.Add("Expense description is required.");

            if (expense.Amount < 0)
                errors.Add(
                    $"Expense '{expense.Description}' has a negative amount.");

            ValidateCurrency(
                expense.CurrencyCode,
                $"Expense '{expense.Description}' currency",
                errors);

            if (startDate.HasValue && expense.ExpenseDate < startDate.Value)
                errors.Add(
                    $"Expense '{expense.Description}' occurs before the trip starts.");

            if (endDate.HasValue && expense.ExpenseDate > endDate.Value)
                errors.Add(
                    $"Expense '{expense.Description}' occurs after the trip ends.");
        }
    }

    private static void ValidateCurrency(
        string? currencyCode,
        string fieldName,
        List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(currencyCode))
        {
            errors.Add($"{fieldName} is required.");
            return;
        }

        var normalized = currencyCode.Trim();

        if (normalized.Length != 3 ||
            normalized.Any(c => c < 'A' || c > 'Z'))
        {
            errors.Add(
                $"{fieldName} must be a three-letter uppercase currency code.");
        }
    }
}
