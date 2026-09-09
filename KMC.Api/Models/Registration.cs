namespace KMC.Api.Models;

public class Registration
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public Event? Event { get; set; }

    public string ParticipantName { get; set; } = string.Empty;
    public string ParticipantEmail { get; set; } = string.Empty;

    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
}
