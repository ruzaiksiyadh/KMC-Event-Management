using System.Security.Claims;
using KMC.Api.DTOs;
using KMC.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KMC.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirst("sub")?.Value
        ?? throw new UnauthorizedAccessException();

    // Public search: GET /api/events?keyword=fair&eventType=Market&fromDate=2026-08-01&toDate=2026-09-01
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<EventDto>>> Search([FromQuery] string? keyword, [FromQuery] string? eventType,
        [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
    {
        var results = await _eventService.SearchAsync(new EventSearchQuery(keyword, eventType, fromDate, toDate));
        return Ok(results);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<EventDto>> GetById(int id)
    {
        var result = await _eventService.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    // Organizer's own events, for the management dashboard.
    [HttpGet("mine")]
    [Authorize]
    public async Task<ActionResult<List<EventDto>>> GetMine()
    {
        var results = await _eventService.GetByOrganizerAsync(CurrentUserId);
        return Ok(results);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<EventDto>> Create(CreateEventDto dto)
    {
        var created = await _eventService.CreateAsync(dto, CurrentUserId);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<ActionResult<EventDto>> Update(int id, UpdateEventDto dto)
    {
        try
        {
            var updated = await _eventService.UpdateAsync(id, dto, CurrentUserId);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deleted = await _eventService.DeleteAsync(id, CurrentUserId);
            return deleted ? NoContent() : NotFound();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
