using KMC.Api.Data;
using KMC.Api.DTOs;
using KMC.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace KMC.Api.Services;

public class EventService : IEventService
{
    private readonly ApplicationDbContext _db;

    public EventService(ApplicationDbContext db)
    {
        _db = db;
    }

    private static EventDto ToDto(Event e) => new(
        e.Id, e.Title, e.Description, e.EventType, e.EventDate, e.Location, e.Capacity,
        e.OrganizerId, e.Organizer?.DisplayName ?? "", e.Registrations?.Count ?? 0
    );

    public async Task<List<EventDto>> SearchAsync(EventSearchQuery query)
    {
        var events = _db.Events
            .Include(e => e.Organizer)
            .Include(e => e.Registrations)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Keyword))
            events = events.Where(e => e.Title.Contains(query.Keyword) || e.Description.Contains(query.Keyword));

        if (!string.IsNullOrWhiteSpace(query.EventType))
            events = events.Where(e => e.EventType == query.EventType);

        if (query.FromDate.HasValue)
            events = events.Where(e => e.EventDate >= query.FromDate.Value);

        if (query.ToDate.HasValue)
            events = events.Where(e => e.EventDate <= query.ToDate.Value);

        var results = await events.OrderBy(e => e.EventDate).ToListAsync();
        return results.Select(ToDto).ToList();
    }

    public async Task<EventDto?> GetByIdAsync(int id)
    {
        var e = await _db.Events
            .Include(e => e.Organizer)
            .Include(e => e.Registrations)
            .FirstOrDefaultAsync(e => e.Id == id);
        return e is null ? null : ToDto(e);
    }

    public async Task<EventDto> CreateAsync(CreateEventDto dto, string organizerId)
    {
        var entity = new Event
        {
            Title = dto.Title,
            Description = dto.Description,
            EventType = dto.EventType,
            EventDate = dto.EventDate,
            Location = dto.Location,
            Capacity = dto.Capacity,
            OrganizerId = organizerId
        };
        _db.Events.Add(entity);
        await _db.SaveChangesAsync();
        await _db.Entry(entity).Reference(e => e.Organizer).LoadAsync();
        return ToDto(entity);
    }

    public async Task<EventDto?> UpdateAsync(int id, UpdateEventDto dto, string organizerId)
    {
        var entity = await _db.Events.Include(e => e.Organizer).Include(e => e.Registrations)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (entity is null) return null;

        // Enforces "only the creator can update their events" from the brief.
        if (entity.OrganizerId != organizerId)
            throw new UnauthorizedAccessException("Only the organizer who created this event can update it.");

        entity.Title = dto.Title;
        entity.Description = dto.Description;
        entity.EventType = dto.EventType;
        entity.EventDate = dto.EventDate;
        entity.Location = dto.Location;
        entity.Capacity = dto.Capacity;

        await _db.SaveChangesAsync();
        return ToDto(entity);
    }

    public async Task<bool> DeleteAsync(int id, string organizerId)
    {
        var entity = await _db.Events.FindAsync(id);
        if (entity is null) return false;

        if (entity.OrganizerId != organizerId)
            throw new UnauthorizedAccessException("Only the organizer who created this event can delete it.");

        _db.Events.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<EventDto>> GetByOrganizerAsync(string organizerId)
    {
        var events = await _db.Events
            .Include(e => e.Organizer)
            .Include(e => e.Registrations)
            .Where(e => e.OrganizerId == organizerId)
            .OrderBy(e => e.EventDate)
            .ToListAsync();
        return events.Select(ToDto).ToList();
    }
}
