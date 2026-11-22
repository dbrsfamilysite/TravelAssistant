
using DBSR.TravelAssistant.Core.AI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DBSR.TravelAssistant.Tests;

public class TravelAssistantApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<ITravelPlanGenerator>();

            services.AddSingleton<ITravelPlanGenerator>(
                new FakeTravelPlanGenerator());
        });
    }
}

public sealed class FakeTravelPlanGenerator : ITravelPlanGenerator
{
    public Task<TravelPlanResponse> GenerateAsync(
        string userRequest,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userRequest);

        var plan = new TravelPlanResponse
        {
            TripName = "Test Family Trip",
            Overview = "A generated test travel plan.",
            CurrencyCode = "USD",
            StartDate = new DateOnly(2026, 12, 1),
            EndDate = new DateOnly(2026, 12, 2),
            NumberOfTravelers = 3,
            BudgetLimit = 2000,
            Assumptions = [],
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
                            Name = "Visit Museum",
                            Category = DBSR.TravelAssistant.Core.Itineraries.ActivityCategory.Museum,
                            Description = "Explore museum exhibits.",
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
                    Category = DBSR.TravelAssistant.Core.Budgets.BudgetCategory.Accommodation,
                    Description = "Hotel estimate",
                    Amount = 300,
                    CurrencyCode = "USD",
                    ExpenseDate = new DateOnly(2026, 12, 1),
                    IsEstimated = true
                }
            ]
        };

        return Task.FromResult(plan);
    }
}
