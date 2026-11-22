using System.Text.Json;
using System.Text.Json.Serialization;

namespace DBSR.TravelAssistant.Core.AI;

public static class TravelPlanJsonSerializer
{
    private static readonly JsonSerializerOptions Options = CreateOptions();

    private static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };

        options.Converters.Add(
            new JsonStringEnumConverter(
                namingPolicy: null,
                allowIntegerValues: false));

        return options;
    }

    public static TravelPlanResponse Deserialize(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);

        var plan = JsonSerializer.Deserialize<TravelPlanResponse>(
            json,
            Options);

        return plan ?? throw new JsonException(
            "AI returned an empty travel plan.");
    }
}
