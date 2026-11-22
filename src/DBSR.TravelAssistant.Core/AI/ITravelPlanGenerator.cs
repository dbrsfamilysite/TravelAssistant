namespace DBSR.TravelAssistant.Core.AI;

public interface ITravelPlanGenerator
{
    Task<TravelPlanResponse> GenerateAsync(
        string userRequest,
        CancellationToken cancellationToken = default);
}
