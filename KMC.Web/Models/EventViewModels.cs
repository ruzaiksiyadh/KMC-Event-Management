namespace KMC.Web.Models;

public class EventViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string EventType { get; set; } = "";
    public DateTime EventDate { get; set; }
    public string Location { get; set; } = "";
    public int Capacity { get; set; }
    public string OrganizerId { get; set; } = "";
    public string OrganizerName { get; set; } = "";
    public int RegisteredCount { get; set; }
}

public class EventSearchViewModel
{
    public string? Keyword { get; set; }
    public string? EventType { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public List<EventViewModel> Results { get; set; } = new();
}

public class CreateEventViewModel
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string EventType { get; set; } = "";
    public DateTime EventDate { get; set; } = DateTime.Today.AddDays(7);
    public string Location { get; set; } = "";
    public int Capacity { get; set; } = 50;
}

public class RegisterParticipantViewModel
{
    public int EventId { get; set; }
    public string EventTitle { get; set; } = "";
    public string ParticipantName { get; set; } = "";
    public string ParticipantEmail { get; set; } = "";
}

public class RegistrationViewModel
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public string ParticipantName { get; set; } = "";
    public string ParticipantEmail { get; set; } = "";
    public DateTime RegisteredAt { get; set; }
}
