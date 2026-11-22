
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DBSR.TravelAssistant.Core.Budgets;
using DBSR.TravelAssistant.Core.Trips;

namespace DBSR.TravelAssistant.Tests;

public class BudgetApiIntegrationTests
    : IClassFixture<TravelAssistantApiFactory>
{
    private readonly HttpClient _client;

    public BudgetApiIntegrationTests(TravelAssistantApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CalculateBudget_ShouldReturnCorrectSummary()
    {
        var tripId = Guid.NewGuid();
        var budgetId = Guid.NewGuid();

        var request = new
        {
            trip = new
            {
                id = tripId,
                name = "Colorado Family Trip",
                startDate = "2026-12-01",
                endDate = "2026-12-03"
            },
            budget = new
            {
                id = budgetId,
                tripId,
                totalEstimatedBudget = 1000m,
                currencyCode = "USD"
            },
            expenses = new[]
            {
                new
                {
                    tripBudgetId = budgetId,
                    category = BudgetCategory.Accommodation,
                    description = "Hotel",
                    amount = 340m,
                    currencyCode = "USD",
                    expenseDate = "2026-12-01",
                    isEstimated = true
                },
                new
                {
                    tripBudgetId = budgetId,
                    category = BudgetCategory.Food,
                    description = "Meals",
                    amount = 150m,
                    currencyCode = "USD",
                    expenseDate = "2026-12-01",
                    isEstimated = true
                },
                new
                {
                    tripBudgetId = budgetId,
                    category = BudgetCategory.Transportation,
                    description = "Rental car and fuel",
                    amount = 100m,
                    currencyCode = "USD",
                    expenseDate = "2026-12-02",
                    isEstimated = true
                },
                new
                {
                    tripBudgetId = budgetId,
                    category = BudgetCategory.Activities,
                    description = "Attraction tickets",
                    amount = 110m,
                    currencyCode = "USD",
                    expenseDate = "2026-12-03",
                    isEstimated = true
                }
            },
            conversions = Array.Empty<object>()
        };

        var response = await _client.PostAsJsonAsync(
            "/api/budget/calculate",
            request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        Assert.Equal(
            700m,
            root.GetProperty("totalExpenses").GetDecimal());

        Assert.Equal(
            300m,
            root.GetProperty("remainingBudget").GetDecimal());

        Assert.False(
            root.GetProperty("isOverBudget").GetBoolean());

        Assert.Equal(
            4,
            root.GetProperty("expenseCount").GetInt32());
    }

    [Fact]
public async Task CalculateBudget_ShouldReturnBadRequest_WhenTripIsMissing()
{
    var request = new
    {
        trip = (object?)null,
        budget = new
        {
            id = Guid.NewGuid(),
            tripId = Guid.NewGuid(),
            totalEstimatedBudget = 1000m,
            currencyCode = "USD"
        },
        expenses = Array.Empty<object>(),
        conversions = Array.Empty<object>()
    };

    var response = await _client.PostAsJsonAsync(
        "/api/budget/calculate",
        request);

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
}


[Fact]
public async Task CalculateBudget_ShouldReturnBadRequest_WhenExpenseAmountIsNegative()
{
    var tripId = Guid.NewGuid();
    var budgetId = Guid.NewGuid();

    var request = new
    {
        trip = new
        {
            id = tripId,
            name = "Colorado Trip",
            startDate = "2026-12-01",
            endDate = "2026-12-03"
        },
        budget = new
        {
            id = budgetId,
            tripId,
            totalEstimatedBudget = 1000m,
            currencyCode = "USD"
        },
        expenses = new[]
        {
            new
            {
                tripBudgetId = budgetId,
                category = BudgetCategory.Food,
                description = "Negative expense",
                amount = -50m,
                currencyCode = "USD",
                expenseDate = "2026-12-01",
                isEstimated = true
            }
        },
        conversions = Array.Empty<object>()
    };

    var response = await _client.PostAsJsonAsync(
        "/api/budget/calculate", request);

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
}

[Fact]
public async Task CalculateBudget_ShouldReturnBadRequest_WhenExpenseIsOutsideTripDates()
{
    var tripId = Guid.NewGuid();
    var budgetId = Guid.NewGuid();

    var request = new
    {
        trip = new
        {
            id = tripId,
            name = "Colorado Trip",
            startDate = "2026-12-01",
            endDate = "2026-12-03"
        },
        budget = new
        {
            id = budgetId,
            tripId,
            totalEstimatedBudget = 1000m,
            currencyCode = "USD"
        },
        expenses = new[]
        {
            new
            {
                tripBudgetId = budgetId,
                category = BudgetCategory.Food,
                description = "Expense outside trip",
                amount = 100m,
                currencyCode = "USD",
                expenseDate = "2026-12-05",
                isEstimated = true
            }
        },
        conversions = Array.Empty<object>()
    };

    var response = await _client.PostAsJsonAsync(
        "/api/budget/calculate", request);

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
}

[Fact]
public async Task CalculateBudget_ShouldReturnBadRequest_WhenCurrencyConversionIsMissing()
{
    var tripId = Guid.NewGuid();
    var budgetId = Guid.NewGuid();

    var request = new
    {
        trip = new
        {
            id = tripId,
            name = "Colorado Trip",
            startDate = "2026-12-01",
            endDate = "2026-12-03"
        },
        budget = new
        {
            id = budgetId,
            tripId,
            totalEstimatedBudget = 1000m,
            currencyCode = "USD"
        },
        expenses = new[]
        {
            new
            {
                tripBudgetId = budgetId,
                category = BudgetCategory.Food,
                description = "Meals in Europe",
                amount = 100m,
                currencyCode = "EUR",
                expenseDate = "2026-12-01",
                isEstimated = true
            }
        },
        conversions = Array.Empty<object>()
    };

    var response = await _client.PostAsJsonAsync(
        "/api/budget/calculate", request);

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
}

[Fact]
public async Task CalculateBudget_ShouldReturnOverBudgetSummary_WhenExpensesExceedBudget()
{
    var tripId = Guid.NewGuid();
    var budgetId = Guid.NewGuid();

    var request = new
    {
        trip = new
        {
            id = tripId,
            name = "Colorado Trip",
            startDate = "2026-12-01",
            endDate = "2026-12-03"
        },
        budget = new
        {
            id = budgetId,
            tripId,
            totalEstimatedBudget = 1000m,
            currencyCode = "USD"
        },
        expenses = new[]
        {
            new
            {
                tripBudgetId = budgetId,
                category = BudgetCategory.Accommodation,
                description = "Hotel",
                amount = 1200m,
                currencyCode = "USD",
                expenseDate = "2026-12-01",
                isEstimated = true
            }
        },
        conversions = Array.Empty<object>()
    };

    var response = await _client.PostAsJsonAsync(
        "/api/budget/calculate", request);

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    var json = await response.Content.ReadAsStringAsync();

    using var document = JsonDocument.Parse(json);
    var root = document.RootElement;

    Assert.True(root.GetProperty("isOverBudget").GetBoolean());

    Assert.Equal(
        200m,
        root.GetProperty("amountOverBudget").GetDecimal());

    Assert.Equal(
        0m,
        root.GetProperty("remainingBudget").GetDecimal());
}

}
