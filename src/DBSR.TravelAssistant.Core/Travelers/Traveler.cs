
namespace DBSR.TravelAssistant.Core.Travelers;

public class Traveler
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public TravelerType TravelerType { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<FamilyMember> FamilyMembers { get; set; } = [];
}
