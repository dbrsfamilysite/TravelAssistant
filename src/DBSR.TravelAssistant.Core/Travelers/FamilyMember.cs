namespace DBSR.TravelAssistant.Core.Travelers;

public class FamilyMember
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TravelerId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public DateOnly? DateOfBirth { get; set; }

    public FamilyRelationship Relationship { get; set; } = FamilyRelationship.Other;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
