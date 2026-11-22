
namespace DBSR.TravelAssistant.Core.TravelInsurance;

public class TravelInsuranceBooking
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TravelInsurancePolicyId { get; set; }

    public string? BookingReference { get; set; }

    public DateTime? BookingDate { get; set; }

    public decimal PremiumPaid { get; set; }

    public string CurrencyCode { get; set; } = "USD";

    public string? PaymentReference { get; set; }

    public DateTime? CancellationDate { get; set; }

    public decimal? RefundAmount { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

