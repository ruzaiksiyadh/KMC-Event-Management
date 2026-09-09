namespace KMC.Api.DTOs;

public record RegisterParticipantDto(string ParticipantName, string ParticipantEmail);

public record RegistrationDto(
    int Id,
    int EventId,
    string ParticipantName,
    string ParticipantEmail,
    DateTime RegisteredAt
);
