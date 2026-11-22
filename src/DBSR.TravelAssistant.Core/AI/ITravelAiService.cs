namespace DBSR.TravelAssistant.Core.AI;

public interface ITravelAiService
{
    Task<string> GenerateAsync(
        string prompt,
        CancellationToken cancellationToken = default);
}
