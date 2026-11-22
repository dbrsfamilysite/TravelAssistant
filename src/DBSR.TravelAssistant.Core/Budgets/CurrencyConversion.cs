namespace DBSR.TravelAssistant.Core.Budgets;

public class CurrencyConversion
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string FromCurrencyCode { get; set; } = string.Empty;

    public string ToCurrencyCode { get; set; } = string.Empty;

    public decimal ExchangeRate { get; set; }

    public DateTime RateDate { get; set; }

    public string? RateSource { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
