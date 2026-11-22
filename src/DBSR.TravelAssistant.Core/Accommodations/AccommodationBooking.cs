namespace DBSR.TravelAssistant.Core.Accommodations;

public class AccommodationBooking
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TripId { get; set; }

    public Guid AccommodationId { get; set; }

    public Guid? AccommodationRoomId { get; set; }

    public AccommodationBookingStatus Status { get; set; } = AccommodationBookingStatus.Planned;

    public DateOnly CheckInDate { get; set; }

    public DateOnly CheckOutDate { get; set; }

    public int NumberOfAdults { get; set; }

    public int NumberOfChildren { get; set; }

    public int NumberOfRooms { get; set; } = 1;

    public string? BookingReference { get; set; }

    public string? BookingProvider { get; set; }

    public decimal? EstimatedTotalCost { get; set; }

    public decimal? ActualTotalCost { get; set; }

    public string? CurrencyCode { get; set; }

    public string? SpecialRequests { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}