
namespace DBSR.TravelAssistant.Core.Budgets;

public class TripBudget
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TripId { get; set; }

    public decimal TotalEstimatedBudget { get; set; }

    public string CurrencyCode { get; set; } = "USD";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
