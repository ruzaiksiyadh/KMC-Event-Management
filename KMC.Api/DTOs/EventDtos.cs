namespace KMC.Api.DTOs;

public record EventDto(
    int Id,
    string Title,
    string Description,
    string EventType,
    DateTime EventDate,
    string Location,
    int Capacity,
    string OrganizerId,
    string OrganizerName,
    int RegisteredCount
);

public record CreateEventDto(
    string Title,
    string Description,
    string EventType,
    DateTime EventDate,
    string Location,
    int Capacity
);

public record UpdateEventDto(
    string Title,
    string Description,
    string EventType,
    DateTime EventDate,
    string Location,
    int Capacity
);

// Optional filters for the public search endpoint (date, type, keyword).
public record EventSearchQuery(
    string? Keyword,
    string? EventType,
    DateTime? FromDate,
    DateTime? ToDate
);
