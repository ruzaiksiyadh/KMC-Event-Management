namespace KMC.Api.Models;

public class Event
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // e.g. "Music", "Sports", "Community", "Market" - used for the public search/filter requirement.
    public string EventType { get; set; } = string.Empty;

    public DateTime EventDate { get; set; }
    public string Location { get; set; } = string.Empty;

    public int Capacity { get; set; }

    // Only this organizer may update/delete the event (requirement 1 in the brief).
    public string OrganizerId { get; set; } = string.Empty;
    public ApplicationUser? Organizer { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
}
