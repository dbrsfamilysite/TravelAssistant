
using System.Net.Http.Json;
using System.Text.Json;
using DBSR.TravelAssistant.Core.AI;

namespace DBSR.TravelAssistant.Infrastructure.AI;

public sealed class OllamaTravelAiService : ITravelAiService
{
    private readonly HttpClient _httpClient;

    public OllamaTravelAiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GenerateAsync(
        string prompt,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prompt);

        var request = new
        {
            model = "qwen3.5:9b",
            prompt,
            stream = false,
            think = false,
            options = new
            {
                num_predict = 4096,
                temperature = 0.3
            }
        };

        using var response = await _httpClient.PostAsJsonAsync(
            "/api/generate",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        await using var responseStream =
            await response.Content.ReadAsStreamAsync(cancellationToken);

        using var jsonDocument = await JsonDocument.ParseAsync(
            responseStream,
            cancellationToken: cancellationToken);

        if (!jsonDocument.RootElement.TryGetProperty(
                "response", out var responseElement))
        {
            throw new InvalidOperationException(
                "Ollama returned a response without generated text.");
        }

        return responseElement.GetString() ?? string.Empty;
    }
}
