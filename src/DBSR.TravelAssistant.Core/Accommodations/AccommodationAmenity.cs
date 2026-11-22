namespace DBSR.TravelAssistant.Core.Accommodations;

public class AccommodationAmenity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid AccommodationId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsAvailable { get; set; } = true;

    public bool IsFree { get; set; }

    public decimal? AdditionalCost { get; set; }

    public string? CurrencyCode { get; set; }
}