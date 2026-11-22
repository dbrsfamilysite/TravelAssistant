using DBSR.TravelAssistant.Core.Budgets;
using DBSR.TravelAssistant.Core.Itineraries;

namespace DBSR.TravelAssistant.Core.AI;

public sealed class TravelPlanMapper
{
    private readonly TravelPlanValidator _validator;

    public TravelPlanMapper(TravelPlanValidator validator)
    {
        _validator = validator;
    }

    public Itinerary MapToItinerary(
        TravelPlanResponse plan,
        Guid tripId)
    {
        ArgumentNullException.ThrowIfNull(plan);

        if (tripId == Guid.Empty)
            throw new ArgumentException("Trip ID cannot be empty.", nameof(tripId));

        var errors = _validator.Validate(plan);

        if (errors.Count > 0)
        {
            throw new ArgumentException(
                "Travel plan validation failed: " + string.Join(" ", errors));
        }

        return new Itinerary
        {
            Id = Guid.NewGuid(),
            TripId = tripId,
            Name = plan.TripName,
            Description = plan.Overview,
            IsAiGenerated = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public List<ItineraryDay> MapToItineraryDays(
        TravelPlanResponse plan,
        Guid itineraryId)
    {
        ArgumentNullException.ThrowIfNull(plan);

        if (itineraryId == Guid.Empty)
            throw new ArgumentException(
                "Itinerary ID cannot be empty.", nameof(itineraryId));

        var errors = _validator.Validate(plan);

        if (errors.Count > 0)
        {
            throw new ArgumentException(
                "Travel plan validation failed: " + string.Join(" ", errors));
        }

        return plan.Days
            .OrderBy(day => day.DayNumber)
            .Select(day => new ItineraryDay
            {
                Id = Guid.NewGuid(),
                ItineraryId = itineraryId,
                DayNumber = day.DayNumber,
                Date = day.Date,
                Title = day.Title,
                Notes = day.Notes
            })
            .ToList();
    }

    public List<Activity> MapToActivities(
        TravelPlanResponse plan,
        IReadOnlyDictionary<int, Guid> itineraryDayIds)
    {
        ArgumentNullException.ThrowIfNull(plan);
        ArgumentNullException.ThrowIfNull(itineraryDayIds);

        var errors = _validator.Validate(plan);

        if (errors.Count > 0)
        {
            throw new ArgumentException(
                "Travel plan validation failed: " + string.Join(" ", errors));
        }

        var activities = new List<Activity>();

        foreach (var day in plan.Days)
        {
            if (!itineraryDayIds.TryGetValue(day.DayNumber, out var dayId) ||
                dayId == Guid.Empty)
            {
                throw new ArgumentException(
                    $"A valid itinerary day ID is required for day {day.DayNumber}.");
            }

            activities.AddRange(day.Activities.Select(activity => new Activity
            {
                Id = Guid.NewGuid(),
                ItineraryDayId = dayId,
                PlaceId = null,
                Name = activity.Name,
                Category = activity.Category,
                Description = activity.Description,
                StartTime = activity.StartTime,
                EndTime = activity.EndTime,
                EstimatedCost = activity.EstimatedCost,
                CurrencyCode = activity.CurrencyCode,
                SequenceNumber = activity.SequenceNumber
            }));
        }

        return activities;
    }

    public List<TripExpense> MapToExpenses(
        TravelPlanResponse plan,
        Guid tripBudgetId)
    {
        ArgumentNullException.ThrowIfNull(plan);

        if (tripBudgetId == Guid.Empty)
            throw new ArgumentException(
                "Trip budget ID cannot be empty.", nameof(tripBudgetId));

        var errors = _validator.Validate(plan);

        if (errors.Count > 0)
        {
            throw new ArgumentException(
                "Travel plan validation failed: " + string.Join(" ", errors));
        }

        return plan.Expenses
            .Select(expense => new TripExpense
            {
                Id = Guid.NewGuid(),
                TripBudgetId = tripBudgetId,
                Category = expense.Category,
                Description = expense.Description,
                Amount = expense.Amount,
                CurrencyCode = expense.CurrencyCode,
                ExpenseDate = expense.ExpenseDate,
                IsEstimated = true,
                CreatedAt = DateTime.UtcNow
            })
            .ToList();
    }
}