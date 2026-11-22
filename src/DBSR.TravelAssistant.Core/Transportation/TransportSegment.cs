namespace DBSR.TravelAssistant.Core.Transportation;

public class TransportSegment
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TripId { get; set; }

    public TransportMode Mode { get; set; }

    public TransportSeatClass? SeatClass { get; set; }

    public TransportBookingStatus BookingStatus { get; set; } = TransportBookingStatus.Planned;

    public Guid? TransportProviderId { get; set; }
    
    public Guid FromLocationId { get; set; }

    public Guid ToLocationId { get; set; }

    public DateTime? DepartureDateTime { get; set; }

    public DateTime? ArrivalDateTime { get; set; }

    public string? BookingReference { get; set; }

    public decimal? EstimatedCost { get; set; }

    public decimal? ActualCost { get; set; }

    public string? CurrencyCode { get; set; }

    public string? Notes { get; set; }
}