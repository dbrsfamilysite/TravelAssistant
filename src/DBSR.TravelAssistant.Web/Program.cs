using System.Text.Json.Serialization;
using DBSR.TravelAssistant.Core.Trips;
using DBSR.TravelAssistant.Core.Budgets;
using DBSR.TravelAssistant.Core.AI;
using DBSR.TravelAssistant.Infrastructure.AI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(
        new JsonStringEnumConverter());
});

builder.Services.AddSingleton<TripBudgetCalculator>();

builder.Services.AddSingleton<TravelPlanValidator>();
builder.Services.AddScoped<ITravelPlanGenerator, OllamaTravelPlanGenerator>();

builder.Services.AddHttpClient<ITravelAiService, OllamaTravelAiService>(
    client =>
    {
        client.BaseAddress = new Uri("http://localhost:11434");
        client.Timeout = TimeSpan.FromMinutes(5);
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild",
    "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast(
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();

    return forecast;
})
.WithName("GetWeatherForecast");

app.MapPost("/api/ai/generate", async (
    GenerateAiRequest request,
    ITravelAiService aiService,
    CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(request.Prompt))
    {
        return Results.BadRequest(new
        {
            error = "Prompt cannot be empty."
        });
    }

    var travelPrompt = TravelPromptBuilder.Build(request.Prompt);

    var result = await aiService.GenerateAsync(
        travelPrompt,
        cancellationToken);

    return Results.Ok(new
    {
        prompt = request.Prompt,
        response = result,
        model = "qwen3.5:9b",
        provider = "Ollama (Local)"
    });
})
.WithName("GenerateTravelAiResponse");


app.MapPost("/api/travel-plans/generate", async (
    GenerateTravelPlanRequest request,
    ITravelPlanGenerator generator,
    CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(request.UserRequest))
    {
        return Results.BadRequest(new
        {
            error = "Travel request cannot be empty."
        });
    }

    try
    {
        var plan = await generator.GenerateAsync(
            request.UserRequest,
            cancellationToken);

        return Results.Ok(plan);
    }
    catch (System.Text.Json.JsonException ex)
    {
        return Results.UnprocessableEntity(new
        {
            error = "AI returned an invalid JSON travel plan.",
            details = ex.Message
        });
    }
    catch (InvalidOperationException ex)
    {
        return Results.UnprocessableEntity(new
        {
            error = "AI generated a travel plan that failed validation.",
            details = ex.Message
        });
    }
})
.WithName("GenerateStructuredTravelPlan");


app.MapPost("/api/budget/calculate", (
    BudgetCalculationRequest request,
    TripBudgetCalculator calculator) =>
{
    if (request.Trip is null || request.Budget is null)
    {
        return Results.BadRequest(new
        {
            error = "Trip and budget are required."
        });
    }

    if (request.Expenses is null || request.Conversions is null)
    {
        return Results.BadRequest(new
        {
            error = "Expenses and conversions cannot be null."
        });
    }

    try
    {
        var summary = calculator.Calculate(
            request.Trip,
            request.Budget,
            request.Expenses,
            request.Conversions);

        return Results.Ok(summary);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("CalculateTripBudget");

app.Run();

record GenerateAiRequest(string Prompt);

record GenerateTravelPlanRequest(string UserRequest);

record BudgetCalculationRequest(
    Trip? Trip,
    TripBudget? Budget,
    List<TripExpense>? Expenses,
    List<CurrencyConversion>? Conversions);

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public partial class Program
{
}
