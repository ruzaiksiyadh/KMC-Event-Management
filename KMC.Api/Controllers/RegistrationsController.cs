using System.Security.Claims;
using KMC.Api.DTOs;
using KMC.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KMC.Api.Controllers;

[ApiController]
[Route("api/events/{eventId:int}/registrations")]
public class RegistrationsController : ControllerBase
{
    private readonly IRegistrationService _registrationService;

    public RegistrationsController(IRegistrationService registrationService)
    {
        _registrationService = registrationService;
    }

    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirst("sub")?.Value
        ?? throw new UnauthorizedAccessException();

    // Any member of the public can register - no auth required.
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<RegistrationDto>> Register(int eventId, RegisterParticipantDto dto)
    {
        try
        {
            var result = await _registrationService.RegisterAsync(eventId, dto);
            return result is null ? NotFound() : Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    // Only the organizer who owns the event can see who registered.
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<List<RegistrationDto>>> GetForEvent(int eventId)
    {
        try
        {
            var results = await _registrationService.GetForEventAsync(eventId, CurrentUserId);
            return results is null ? NotFound() : Ok(results);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
