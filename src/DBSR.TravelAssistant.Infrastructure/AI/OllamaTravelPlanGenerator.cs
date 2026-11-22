using DBSR.TravelAssistant.Core.AI;

namespace DBSR.TravelAssistant.Infrastructure.AI;

public sealed class OllamaTravelPlanGenerator : ITravelPlanGenerator
{
    private readonly ITravelAiService _aiService;
    private readonly TravelPlanValidator _validator;

    public OllamaTravelPlanGenerator(
        ITravelAiService aiService,
        TravelPlanValidator validator)
    {
        _aiService = aiService;
        _validator = validator;
    }

    public async Task<TravelPlanResponse> GenerateAsync(
        string userRequest,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userRequest);

        var prompt = TravelPromptBuilder.Build(userRequest);

        var json = await _aiService.GenerateAsync(
            prompt,
            cancellationToken);

        var plan = TravelPlanJsonSerializer.Deserialize(json);

        var errors = _validator.Validate(plan);

        if (errors.Count > 0)
        {
            throw new InvalidOperationException(
                "Generated travel plan failed validation: " +
                string.Join(" ", errors));
        }

        return plan;
    }
}
