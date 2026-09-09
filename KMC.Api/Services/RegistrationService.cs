using KMC.Api.Data;
using KMC.Api.DTOs;
using KMC.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace KMC.Api.Services;

public class RegistrationService : IRegistrationService
{
    private readonly ApplicationDbContext _db;

    public RegistrationService(ApplicationDbContext db)
    {
        _db = db;
    }

    private static RegistrationDto ToDto(Registration r) =>
        new(r.Id, r.EventId, r.ParticipantName, r.ParticipantEmail, r.RegisteredAt);

    public async Task<RegistrationDto?> RegisterAsync(int eventId, RegisterParticipantDto dto)
    {
        var ev = await _db.Events.Include(e => e.Registrations).FirstOrDefaultAsync(e => e.Id == eventId);
        if (ev is null) return null;

        if (ev.Capacity > 0 && ev.Registrations.Count >= ev.Capacity)
            throw new InvalidOperationException("This event is full.");

        var registration = new Registration
        {
            EventId = eventId,
            ParticipantName = dto.ParticipantName,
            ParticipantEmail = dto.ParticipantEmail
        };
        _db.Registrations.Add(registration);
        await _db.SaveChangesAsync();
        return ToDto(registration);
    }

    public async Task<List<RegistrationDto>?> GetForEventAsync(int eventId, string organizerId)
    {
        var ev = await _db.Events.FindAsync(eventId);
        if (ev is null) return null;

        if (ev.OrganizerId != organizerId)
            throw new UnauthorizedAccessException("Only the organizer who created this event can view its registrations.");

        var regs = await _db.Registrations
            .Where(r => r.EventId == eventId)
            .OrderBy(r => r.RegisteredAt)
            .ToListAsync();
        return regs.Select(ToDto).ToList();
    }
}
