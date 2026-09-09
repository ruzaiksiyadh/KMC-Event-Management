using KMC.Api.DTOs;

namespace KMC.Api.Services;

public interface IRegistrationService
{
    Task<RegistrationDto?> RegisterAsync(int eventId, RegisterParticipantDto dto);
    Task<List<RegistrationDto>?> GetForEventAsync(int eventId, string organizerId);
}
