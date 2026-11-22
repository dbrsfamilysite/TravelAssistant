
using DBSR.TravelAssistant.Core.Budgets;
using DBSR.TravelAssistant.Core.Trips;
using DBSR.TravelAssistant.Core.AI;
using DBSR.TravelAssistant.Core.Itineraries;
using DBSR.TravelAssistant.Infrastructure.AI;

namespace DBSR.TravelAssistant.Tests;

public class TripBudgetCalculatorTests
{
    [Fact]
    public void Calculate_ShouldReturnCorrectTotals_WhenExpensesAreInSameCurrency()
    {
        // Arrange
        var trip = new Trip
        {
            Id = Guid.NewGuid(),
            Name = "Colorado Family Trip",
            StartDate = new DateOnly(2026, 12, 1),
            EndDate = new DateOnly(2026, 12, 3)
        };

        var budget = new TripBudget
        {
            Id = Guid.NewGuid(),
            TripId = trip.Id,
            TotalEstimatedBudget = 1000m,
            CurrencyCode = "USD"
        };

        var expenses = new List<TripExpense>
        {
            new()
            {
                TripBudgetId = budget.Id,
                Category = BudgetCategory.Accommodation,
                Description = "Hotel",
                Amount = 340m,
                CurrencyCode = "USD",
                ExpenseDate = new DateOnly(2026, 12, 1),
                IsEstimated = true
            },
            new()
            {
                TripBudgetId = budget.Id,
                Category = BudgetCategory.Food,
                Description = "Meals",
                Amount = 150m,
                CurrencyCode = "USD",
                ExpenseDate = new DateOnly(2026, 12, 1),
                IsEstimated = true
            },
            new()
            {
                TripBudgetId = budget.Id,
                Category = BudgetCategory.Transportation,
                Description = "Rental car and fuel",
                Amount = 100m,
                CurrencyCode = "USD",
                ExpenseDate = new DateOnly(2026, 12, 2),
                IsEstimated = true
            },
            new()
            {
                TripBudgetId = budget.Id,
                Category = BudgetCategory.Activities,
                Description = "Attraction tickets",
                Amount = 110m,
                CurrencyCode = "USD",
                ExpenseDate = new DateOnly(2026, 12, 3),
                IsEstimated = true
            }
        };

        var conversions = new List<CurrencyConversion>();

        var calculator = new TripBudgetCalculator();

        // Act
        var result = calculator.Calculate(
            trip,
            budget,
            expenses,
            conversions);

        // Assert
        Assert.Equal(700m, result.TotalExpenses);
        Assert.Equal(700m, result.TotalEstimatedExpenses);
        Assert.Equal(0m, result.TotalActualExpenses);
        Assert.Equal(300m, result.RemainingBudget);
        Assert.Equal(0m, result.AmountOverBudget);
        Assert.False(result.IsOverBudget);
        Assert.Equal(70m, result.EstimatedExpensePercentage);
        Assert.Equal(0m, result.ActualExpensePercentage);
        Assert.Equal(4, result.ExpenseCount);
        Assert.Equal(3, result.DailyBreakdown.Count);
        Assert.Equal(4, result.CategoryBreakdown.Count);
    }


    [Fact]
    public void Calculate_ShouldIdentifyOverspending()
    {
        var trip = new Trip
        {
            Id = Guid.NewGuid(),
            StartDate = new DateOnly(2026, 12, 1),
            EndDate = new DateOnly(2026, 12, 3)
        };

        var budget = new TripBudget
        {
            Id = Guid.NewGuid(),
            TripId = trip.Id,
            TotalEstimatedBudget = 1000m,
            CurrencyCode = "USD"
        };

        var expenses = new List<TripExpense>
        {
            new()
            {
                TripBudgetId = budget.Id,
                Category = BudgetCategory.Accommodation,
                Amount = 1200m,
                CurrencyCode = "USD",
                ExpenseDate = new DateOnly(2026, 12, 1),
                IsEstimated = true
            }
        };

        var result = new TripBudgetCalculator().Calculate(
            trip, budget, expenses, []);

        Assert.Equal(1200m, result.TotalExpenses);
        Assert.Equal(200m, result.AmountOverBudget);
        Assert.Equal(0m, result.RemainingBudget);
        Assert.True(result.IsOverBudget);
    }

    [Fact]
    public void Calculate_ShouldConvertForeignCurrencyToBudgetCurrency()
    {
        var trip = new Trip
        {
            Id = Guid.NewGuid(),
            StartDate = new DateOnly(2026, 12, 1),
            EndDate = new DateOnly(2026, 12, 3)
        };

        var budget = new TripBudget
        {
            Id = Guid.NewGuid(),
            TripId = trip.Id,
            TotalEstimatedBudget = 1000m,
            CurrencyCode = "USD"
        };

        var expenses = new List<TripExpense>
        {
            new()
            {
                TripBudgetId = budget.Id,
                Category = BudgetCategory.Food,
                Amount = 100m,
                CurrencyCode = "EUR",
                ExpenseDate = new DateOnly(2026, 12, 1),
                IsEstimated = true
            }
        };

        var conversions = new List<CurrencyConversion>
        {
            new()
            {
                FromCurrencyCode = "EUR",
                ToCurrencyCode = "USD",
                ExchangeRate = 1.10m,
                RateDate = new DateTime(2026, 11, 30)
            }
        };

        var result = new TripBudgetCalculator().Calculate(
            trip, budget, expenses, conversions);

        Assert.Equal(110m, result.TotalExpenses);
        Assert.Equal("USD", result.CurrencyCode);
    }

    [Fact]
    public void Calculate_ShouldRejectMissingCurrencyConversion()
    {
        var trip = new Trip
        {
            Id = Guid.NewGuid(),
            StartDate = new DateOnly(2026, 12, 1),
            EndDate = new DateOnly(2026, 12, 3)
        };

        var budget = new TripBudget
        {
            Id = Guid.NewGuid(),
            TripId = trip.Id,
            TotalEstimatedBudget = 1000m,
            CurrencyCode = "USD"
        };

        var expenses = new List<TripExpense>
        {
            new()
            {
                TripBudgetId = budget.Id,
                Category = BudgetCategory.Food,
                Amount = 100m,
                CurrencyCode = "EUR",
                ExpenseDate = new DateOnly(2026, 12, 1),
                IsEstimated = true
            }
        };

        var exception = Assert.Throws<InvalidOperationException>(() =>
            new TripBudgetCalculator().Calculate(
                trip, budget, expenses, []));

        Assert.Contains("No currency conversion rate", exception.Message);
    }

    [Fact]
    public void Calculate_ShouldRejectExpenseOutsideTripDates()
    {
        var trip = new Trip
        {
            Id = Guid.NewGuid(),
            StartDate = new DateOnly(2026, 12, 1),
            EndDate = new DateOnly(2026, 12, 3)
        };

        var budget = new TripBudget
        {
            Id = Guid.NewGuid(),
            TripId = trip.Id,
            TotalEstimatedBudget = 1000m,
            CurrencyCode = "USD"
        };

        var expenses = new List<TripExpense>
        {
            new()
            {
                TripBudgetId = budget.Id,
                Category = BudgetCategory.Food,
                Amount = 100m,
                CurrencyCode = "USD",
                ExpenseDate = new DateOnly(2026, 12, 5),
                IsEstimated = true
            }
        };

        Assert.Throws<ArgumentException>(() =>
            new TripBudgetCalculator().Calculate(
                trip, budget, expenses, []));
    }

    

}

public class TravelPlanValidatorTests
{
    private readonly TravelPlanValidator _validator = new();

    private static TravelPlanResponse CreateValidPlan()
    {
        return new TravelPlanResponse
        {
            TripName = "Colorado Family Trip",
            Overview = "A family trip exploring Colorado.",
            CurrencyCode = "USD",
            StartDate = new DateOnly(2026, 12, 1),
            EndDate = new DateOnly(2026, 12, 3),
            NumberOfTravelers = 3,
            BudgetLimit = 2000,
            Days =
            [
                new TravelPlanDayResponse
                {
                    DayNumber = 1,
                    Date = new DateOnly(2026, 12, 1),
                    Title = "Explore Denver",
                    Activities =
                    [
                        new TravelPlanActivityResponse
                        {
                            Name = "Visit Denver Museum",
                            Category = ActivityCategory.Museum,
                            StartTime = new TimeOnly(10, 0),
                            EndTime = new TimeOnly(12, 0),
                            EstimatedCost = 50,
                            CurrencyCode = "USD",
                            SequenceNumber = 1
                        }
                    ]
                }
            ],
            Expenses =
            [
                new TravelPlanExpenseResponse
                {
                    Category = BudgetCategory.Activities,
                    Description = "Museum tickets",
                    Amount = 50,
                    CurrencyCode = "USD",
                    ExpenseDate = new DateOnly(2026, 12, 1),
                    IsEstimated = true
                }
            ]
        };
    }

    [Fact]
    public void Validate_ValidPlan_ReturnsNoErrors()
    {
        var plan = CreateValidPlan();

        var errors = _validator.Validate(plan);

        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_InvalidTripDates_ReturnsError()
    {
        var plan = CreateValidPlan();
        plan.StartDate = new DateOnly(2026, 12, 5);
        plan.EndDate = new DateOnly(2026, 12, 1);

        var errors = _validator.Validate(plan);

        Assert.Contains(errors,
            x => x.Contains("Trip start date cannot be after end date."));
    }

    [Fact]
    public void Validate_ZeroTravelers_ReturnsError()
    {
        var plan = CreateValidPlan();
        plan.NumberOfTravelers = 0;

        var errors = _validator.Validate(plan);

        Assert.Contains(errors,
            x => x.Contains("Number of travelers must be greater than zero."));
    }

    [Fact]
    public void Validate_DuplicateDayNumbers_ReturnsError()
    {
        var plan = CreateValidPlan();

        plan.Days.Add(new TravelPlanDayResponse
        {
            DayNumber = 1,
            Date = new DateOnly(2026, 12, 2),
            Title = "Second day"
        });

        var errors = _validator.Validate(plan);

        Assert.Contains(errors,
            x => x.Contains("Duplicate itinerary day number"));
    }

    [Fact]
    public void Validate_InvalidActivityTime_ReturnsError()
    {
        var plan = CreateValidPlan();
        plan.Days[0].Activities[0].StartTime = new TimeOnly(14, 0);
        plan.Days[0].Activities[0].EndTime = new TimeOnly(12, 0);

        var errors = _validator.Validate(plan);

        Assert.Contains(errors,
            x => x.Contains("ends before it starts"));
    }

    [Fact]
    public void Validate_NegativeActivityCost_ReturnsError()
    {
        var plan = CreateValidPlan();
        plan.Days[0].Activities[0].EstimatedCost = -10;

        var errors = _validator.Validate(plan);

        Assert.Contains(errors,
            x => x.Contains("negative estimated cost"));
    }

    [Fact]
    public void Validate_NegativeExpense_ReturnsError()
    {
        var plan = CreateValidPlan();
        plan.Expenses[0].Amount = -50;

        var errors = _validator.Validate(plan);

        Assert.Contains(errors,
            x => x.Contains("negative amount"));
    }

    [Fact]
    public void Validate_InvalidCurrency_ReturnsError()
    {
        var plan = CreateValidPlan();
        plan.CurrencyCode = "US";

        var errors = _validator.Validate(plan);

        Assert.Contains(errors,
            x => x.Contains("three-letter uppercase currency code"));
    }

    [Fact]
    public void Validate_ExpenseOutsideTripDates_ReturnsError()
    {
        var plan = CreateValidPlan();
        plan.Expenses[0].ExpenseDate = new DateOnly(2026, 12, 5);

        var errors = _validator.Validate(plan);

        Assert.Contains(errors,
            x => x.Contains("occurs after the trip ends"));
    }


    [Fact]
    public void Validate_DuplicateActivitySequenceNumbers_ReturnsError()
    {
        var plan = CreateValidPlan();

        plan.Days[0].Activities.Add(new TravelPlanActivityResponse
        {
            Name = "Visit Denver Botanic Gardens",
            Category = ActivityCategory.Nature,
            StartTime = new TimeOnly(13, 0),
            EndTime = new TimeOnly(15, 0),
            EstimatedCost = 30,
            CurrencyCode = "USD",
            SequenceNumber = 1
        });

        var errors = _validator.Validate(plan);

        Assert.Contains(errors,
            x => x.Contains("duplicate activity sequence number"));
    }

    [Fact]
    public void Validate_OverlappingActivities_ReturnsError()
    {
        var plan = CreateValidPlan();

        plan.Days[0].Activities.Add(new TravelPlanActivityResponse
        {
            Name = "Visit Denver Botanic Gardens",
            Category = ActivityCategory.Nature,
            StartTime = new TimeOnly(11, 0),
            EndTime = new TimeOnly(14, 0),
            EstimatedCost = 30,
            CurrencyCode = "USD",
            SequenceNumber = 2
        });

        var errors = _validator.Validate(plan);

        Assert.Contains(errors,
            x => x.Contains("overlap"));
    }

    [Fact]
    public void Validate_SequenceConflictsWithChronologicalOrder_ReturnsError()
    {
        var plan = CreateValidPlan();

        plan.Days[0].Activities.Add(new TravelPlanActivityResponse
        {
            Name = "Visit Denver Botanic Gardens",
            Category = ActivityCategory.Nature,
            StartTime = new TimeOnly(13, 0),
            EndTime = new TimeOnly(15, 0),
            EstimatedCost = 30,
            CurrencyCode = "USD",
            SequenceNumber = 1
        });

        plan.Days[0].Activities[0].SequenceNumber = 2;

        var errors = _validator.Validate(plan);

        Assert.Contains(errors,
            x => x.Contains("sequence does not follow chronological order"));
    }
}

public class TravelPlanMapperTests
{
    private readonly TravelPlanMapper _mapper =
        new(new TravelPlanValidator());

    private static TravelPlanResponse CreateValidPlan()
    {
        return new TravelPlanResponse
        {
            TripName = "Colorado Family Trip",
            Overview = "A family trip exploring Colorado.",
            CurrencyCode = "USD",
            StartDate = new DateOnly(2026, 12, 1),
            EndDate = new DateOnly(2026, 12, 2),
            NumberOfTravelers = 3,
            BudgetLimit = 2000,
            Days =
            [
                new TravelPlanDayResponse
                {
                    DayNumber = 2,
                    Date = new DateOnly(2026, 12, 2),
                    Title = "Explore Denver",
                    Activities =
                    [
                        new TravelPlanActivityResponse
                        {
                            Name = "Visit Museum",
                            Category = ActivityCategory.Museum,
                            EstimatedCost = 50,
                            CurrencyCode = "USD",
                            SequenceNumber = 1
                        }
                    ]
                },
                new TravelPlanDayResponse
                {
                    DayNumber = 1,
                    Date = new DateOnly(2026, 12, 1),
                    Title = "Arrival Day"
                }
            ],
            Expenses =
            [
                new TravelPlanExpenseResponse
                {
                    Category = BudgetCategory.Activities,
                    Description = "Museum tickets",
                    Amount = 50,
                    CurrencyCode = "USD",
                    ExpenseDate = new DateOnly(2026, 12, 2)
                }
            ]
        };
    }

    [Fact]
    public void MapToItinerary_ValidPlan_MapsCorrectly()
    {
        var plan = CreateValidPlan();
        var tripId = Guid.NewGuid();

        var result = _mapper.MapToItinerary(plan, tripId);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(tripId, result.TripId);
        Assert.Equal(plan.TripName, result.Name);
        Assert.Equal(plan.Overview, result.Description);
        Assert.True(result.IsAiGenerated);
    }

    [Fact]
    public void MapToItineraryDays_ValidPlan_MapsInDayOrder()
    {
        var plan = CreateValidPlan();
        var itineraryId = Guid.NewGuid();

        var result = _mapper.MapToItineraryDays(plan, itineraryId);

        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].DayNumber);
        Assert.Equal(2, result[1].DayNumber);
        Assert.All(result, day =>
            Assert.Equal(itineraryId, day.ItineraryId));
        Assert.All(result, day =>
            Assert.NotEqual(Guid.Empty, day.Id));
    }

    [Fact]
    public void MapToActivities_ValidPlan_MapsActivityToCorrectDay()
    {
        var plan = CreateValidPlan();
        var dayId = Guid.NewGuid();

        var dayIds = new Dictionary<int, Guid>
        {
            [1] = Guid.NewGuid(),
            [2] = dayId
        };

        var result = _mapper.MapToActivities(plan, dayIds);

        var activity = Assert.Single(result);

        Assert.Equal(dayId, activity.ItineraryDayId);
        Assert.Equal("Visit Museum", activity.Name);
        Assert.Equal(ActivityCategory.Museum, activity.Category);
        Assert.Equal(50, activity.EstimatedCost);
        Assert.Null(activity.PlaceId);
    }

    [Fact]
    public void MapToExpenses_ValidPlan_MapsAsEstimated()
    {
        var plan = CreateValidPlan();
        var budgetId = Guid.NewGuid();

        var result = _mapper.MapToExpenses(plan, budgetId);

        var expense = Assert.Single(result);

        Assert.NotEqual(Guid.Empty, expense.Id);
        Assert.Equal(budgetId, expense.TripBudgetId);
        Assert.Equal(BudgetCategory.Activities, expense.Category);
        Assert.Equal(50, expense.Amount);
        Assert.Equal("USD", expense.CurrencyCode);
        Assert.True(expense.IsEstimated);
    }

    [Fact]
    public void MapToItinerary_InvalidPlan_ThrowsArgumentException()
    {
        var plan = CreateValidPlan();
        plan.TripName = string.Empty;

        Assert.Throws<ArgumentException>(() =>
            _mapper.MapToItinerary(plan, Guid.NewGuid()));
    }

    [Fact]
    public void MapToActivities_MissingDayId_ThrowsArgumentException()
    {
        var plan = CreateValidPlan();

        var dayIds = new Dictionary<int, Guid>
        {
            [1] = Guid.NewGuid()
        };

        Assert.Throws<ArgumentException>(() =>
            _mapper.MapToActivities(plan, dayIds));
    }
}


public class TravelPlanJsonSerializerTests
{
    [Fact]
    public void Deserialize_ValidJson_ReturnsTravelPlan()
    {
        var json = """
        {
            "TripName": "Colorado Trip",
            "Overview": "Family vacation",
            "CurrencyCode": "USD",
            "StartDate": "2026-12-01",
            "EndDate": "2026-12-02",
            "NumberOfTravelers": 3,
            "BudgetLimit": 2000,
            "Assumptions": [],
            "Days": [
                {
                    "DayNumber": 1,
                    "Date": "2026-12-01",
                    "Title": "Explore Denver",
                    "Notes": null,
                    "Activities": [
                        {
                            "Name": "Visit Museum",
                            "Category": "Museum",
                            "Description": "Explore exhibits",
                            "StartTime": "10:00:00",
                            "EndTime": "12:00:00",
                            "EstimatedCost": 50,
                            "CurrencyCode": "USD",
                            "SequenceNumber": 1
                        }
                    ]
                }
            ],
            "Expenses": [
                {
                    "Category": "Accommodation",
                    "Description": "Hotel",
                    "Amount": 300,
                    "CurrencyCode": "USD",
                    "ExpenseDate": "2026-12-01",
                    "IsEstimated": true
                }
            ]
        }
        """;

        var result = TravelPlanJsonSerializer.Deserialize(json);

        Assert.Equal("Colorado Trip", result.TripName);
        Assert.Equal(new DateOnly(2026, 12, 1), result.StartDate);
        Assert.Equal(ActivityCategory.Museum,
            result.Days[0].Activities[0].Category);
        Assert.Equal(BudgetCategory.Accommodation,
            result.Expenses[0].Category);
        Assert.Equal(new TimeOnly(10, 0),
            result.Days[0].Activities[0].StartTime);
    }

    [Fact]
    public void Deserialize_CamelCaseJson_ReturnsTravelPlan()
    {
        var json = """
        {
            "tripName": "Test Trip",
            "overview": "Test overview",
            "currencyCode": "USD",
            "numberOfTravelers": 2,
            "assumptions": [],
            "days": [
                {
                    "dayNumber": 1,
                    "date": "2026-12-01",
                    "title": "Arrival",
                    "activities": []
                }
            ],
            "expenses": []
        }
        """;

        var result = TravelPlanJsonSerializer.Deserialize(json);

        Assert.Equal("Test Trip", result.TripName);
        Assert.Equal(2, result.NumberOfTravelers);
        Assert.Single(result.Days);
    }

    [Fact]
    public void Deserialize_InvalidActivityEnum_ThrowsJsonException()
    {
        var json = """
        {
            "TripName": "Test",
            "Overview": "Test",
            "CurrencyCode": "USD",
            "NumberOfTravelers": 2,
            "Days": [
                {
                    "DayNumber": 1,
                    "Date": "2026-12-01",
                    "Title": "Test",
                    "Activities": [
                        {
                            "Name": "Test",
                            "Category": "InvalidCategory"
                        }
                    ]
                }
            ],
            "Expenses": []
        }
        """;

        Assert.Throws<System.Text.Json.JsonException>(() =>
            TravelPlanJsonSerializer.Deserialize(json));
    }

    [Fact]
    public void Deserialize_NumericEnum_ThrowsJsonException()
    {
        var json = """
        {
            "TripName": "Test",
            "Overview": "Test",
            "CurrencyCode": "USD",
            "NumberOfTravelers": 2,
            "Days": [
                {
                    "DayNumber": 1,
                    "Date": "2026-12-01",
                    "Title": "Test",
                    "Activities": [
                        {
                            "Name": "Test",
                            "Category": 3
                        }
                    ]
                }
            ],
            "Expenses": []
        }
        """;

        Assert.Throws<System.Text.Json.JsonException>(() =>
            TravelPlanJsonSerializer.Deserialize(json));
    }

    [Fact]
    public void Deserialize_MalformedJson_ThrowsJsonException()
    {
        var json = "{ invalid json }";

        Assert.Throws<System.Text.Json.JsonException>(() =>
            TravelPlanJsonSerializer.Deserialize(json));
    }

    [Fact]
    public void Deserialize_EmptyJson_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            TravelPlanJsonSerializer.Deserialize(string.Empty));
    }
}


public class OllamaTravelPlanGeneratorTests
{
    private const string ValidJson = """
    {
        "TripName": "Colorado Family Trip",
        "Overview": "A family trip to Colorado.",
        "CurrencyCode": "USD",
        "StartDate": "2026-12-01",
        "EndDate": "2026-12-02",
        "NumberOfTravelers": 3,
        "BudgetLimit": 2000,
        "Assumptions": [],
        "Days": [
            {
                "DayNumber": 1,
                "Date": "2026-12-01",
                "Title": "Explore Denver",
                "Notes": null,
                "Activities": [
                    {
                        "Name": "Visit Museum",
                        "Category": "Museum",
                        "Description": "Explore exhibits.",
                        "StartTime": "10:00:00",
                        "EndTime": "12:00:00",
                        "EstimatedCost": 50,
                        "CurrencyCode": "USD",
                        "SequenceNumber": 1
                    }
                ]
            }
        ],
        "Expenses": [
            {
                "Category": "Accommodation",
                "Description": "Hotel estimate",
                "Amount": 300,
                "CurrencyCode": "USD",
                "ExpenseDate": "2026-12-01",
                "IsEstimated": true
            }
        ]
    }
    """;

    [Fact]
    public async Task GenerateAsync_ValidResponse_ReturnsTravelPlan()
    {
        var fakeAi = new FakeTravelAiService(ValidJson);
        var generator = new OllamaTravelPlanGenerator(
            fakeAi,
            new TravelPlanValidator());

        var result = await generator.GenerateAsync(
            "Plan a two-day Colorado family trip.");

        Assert.Equal("Colorado Family Trip", result.TripName);
        Assert.Equal(3, result.NumberOfTravelers);
        Assert.Single(result.Days);
        Assert.Contains(
            "Plan a two-day Colorado family trip.",
            fakeAi.LastPrompt);
    }

    [Fact]
    public async Task GenerateAsync_InvalidJson_ThrowsJsonException()
    {
        var fakeAi = new FakeTravelAiService("This is not JSON.");
        var generator = new OllamaTravelPlanGenerator(
            fakeAi,
            new TravelPlanValidator());

        await Assert.ThrowsAsync<System.Text.Json.JsonException>(
            () => generator.GenerateAsync("Plan a trip."));
    }

    [Fact]
    public async Task GenerateAsync_InvalidTravelPlan_ThrowsInvalidOperationException()
    {
        var invalidJson = """
        {
            "TripName": "",
            "Overview": "",
            "CurrencyCode": "USD",
            "NumberOfTravelers": 0,
            "Assumptions": [],
            "Days": [],
            "Expenses": []
        }
        """;

        var fakeAi = new FakeTravelAiService(invalidJson);
        var generator = new OllamaTravelPlanGenerator(
            fakeAi,
            new TravelPlanValidator());

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => generator.GenerateAsync("Plan a trip."));

        Assert.Contains("failed validation", exception.Message);
    }

    [Fact]
    public async Task GenerateAsync_EmptyRequest_ThrowsArgumentException()
    {
        var fakeAi = new FakeTravelAiService(ValidJson);
        var generator = new OllamaTravelPlanGenerator(
            fakeAi,
            new TravelPlanValidator());

        await Assert.ThrowsAsync<ArgumentException>(
            () => generator.GenerateAsync(" "));

        Assert.Null(fakeAi.LastPrompt);
    }

    private sealed class FakeTravelAiService : ITravelAiService
    {
        private readonly string _response;

        public string? LastPrompt { get; private set; }

        public FakeTravelAiService(string response)
        {
            _response = response;
        }

        public Task<string> GenerateAsync(
            string prompt,
            CancellationToken cancellationToken = default)
        {
            LastPrompt = prompt;
            return Task.FromResult(_response);
        }
    }
}
