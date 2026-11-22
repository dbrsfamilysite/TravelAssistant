namespace DBSR.TravelAssistant.Core.Accommodations;

public class AccommodationRoom
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid AccommodationId { get; set; }

    public string RoomName { get; set; } = string.Empty;

    public int MaxOccupancy { get; set; }

    public int BedCount { get; set; }

    public string? BedConfiguration { get; set; }

    public decimal? EstimatedNightlyRate { get; set; }

    public string? CurrencyCode { get; set; }

    public bool HasPrivateBathroom { get; set; }

    public bool HasAirConditioning { get; set; }

    public bool IsAccessible { get; set; }

    public string? Description { get; set; }
}