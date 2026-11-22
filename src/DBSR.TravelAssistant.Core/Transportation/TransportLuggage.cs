namespace DBSR.TravelAssistant.Core.Transportation;

public class TransportLuggage
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TransportPassengerId { get; set; }

    public int CheckedBagCount { get; set; }

    public int CarryOnBagCount { get; set; }

    public decimal? TotalWeightKg { get; set; }

    public decimal? AdditionalBaggageCost { get; set; }

    public string? CurrencyCode { get; set; }

    public string? Notes { get; set; }
}