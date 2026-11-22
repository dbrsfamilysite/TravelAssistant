namespace DBSR.TravelAssistant.Core.Transportation;

public class TransportPassenger
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TransportSegmentId { get; set; }

    public Guid TravelerId { get; set; }

    public string? TicketNumber { get; set; }

    public string? SeatNumber { get; set; }

    public string? PassengerType { get; set; }

    public decimal? PassengerCost { get; set; }

    public string? CurrencyCode { get; set; }

    public string? Notes { get; set; }
}