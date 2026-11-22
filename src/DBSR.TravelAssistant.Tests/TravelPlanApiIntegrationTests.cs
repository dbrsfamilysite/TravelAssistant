
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace DBSR.TravelAssistant.Tests;

public class TravelPlanApiIntegrationTests
{
    [Fact]
    public async Task GenerateTravelPlan_ValidRequest_ReturnsStructuredPlan()
    {
        using var factory = new TravelAssistantApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/travel-plans/generate",
            new
            {
                userRequest = "Plan a two-day family trip to Colorado."
            });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(json);

        var root = document.RootElement;

        Assert.Equal(
            "Test Family Trip",
            root.GetProperty("tripName").GetString());

        Assert.Equal(
            3,
            root.GetProperty("numberOfTravelers").GetInt32());

        Assert.Single(root.GetProperty("days").EnumerateArray());

        Assert.Single(root.GetProperty("expenses").EnumerateArray());
    }

    [Fact]
    public async Task GenerateTravelPlan_EmptyRequest_ReturnsBadRequest()
    {
        using var factory = new TravelAssistantApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/travel-plans/generate",
            new
            {
                userRequest = ""
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GenerateTravelPlan_WhitespaceRequest_ReturnsBadRequest()
    {
        using var factory = new TravelAssistantApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/travel-plans/generate",
            new
            {
                userRequest = "   "
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GenerateTravelPlan_MissingRequestBody_ReturnsBadRequest()
    {
        using var factory = new TravelAssistantApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsync(
            "/api/travel-plans/generate",
            null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
