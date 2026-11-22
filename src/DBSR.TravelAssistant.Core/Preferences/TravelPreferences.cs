namespace DBSR.TravelAssistant.Core.Preferences;

public class TravelPreferences
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TravelerId { get; set; }

    public TravelStyle? TravelStyle { get; set; }

    public AccommodationPreference? Accommodation { get; set; }

    public FoodPreference? Food { get; set; }

    public TravelPace? PreferredPace { get; set; }

    public TransportationPreference? Transportation { get; set; }

    public List<TravelInterest> Interests { get; set; } = [];

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
