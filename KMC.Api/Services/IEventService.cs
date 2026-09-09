using KMC.Api.DTOs;

namespace KMC.Api.Services;

public interface IEventService
{
    Task<List<EventDto>> SearchAsync(EventSearchQuery query);
    Task<EventDto?> GetByIdAsync(int id);
    Task<EventDto> CreateAsync(CreateEventDto dto, string organizerId);

    // Returns null if not found, throws UnauthorizedAccessException if caller isn't the owner.
    Task<EventDto?> UpdateAsync(int id, UpdateEventDto dto, string organizerId);
    Task<bool> DeleteAsync(int id, string organizerId);
    Task<List<EventDto>> GetByOrganizerAsync(string organizerId);
}
