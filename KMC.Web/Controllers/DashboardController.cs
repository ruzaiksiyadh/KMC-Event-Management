using System.Security.Claims;
using KMC.Web.Models;
using KMC.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KMC.Web.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class DashboardController : Controller
{
    private readonly ApiClient _apiClient;

    public DashboardController(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    // Attach the organizer's JWT (stored in the auth cookie) to every API call this controller makes.
    private void AttachToken() => _apiClient.SetToken(User.FindFirst("access_token")?.Value);

    // The signed-in organizer's own user id - compared against each event's OrganizerId
    // so one organizer can never edit, view registrations for, or delete another's event.
    private string? CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        AttachToken();
        var myEvents = await _apiClient.GetMyEventsAsync();
        return View(myEvents);
    }

    [HttpGet]
    public IActionResult Create() => View(new CreateEventViewModel());

    [HttpPost]
    public async Task<IActionResult> Create(CreateEventViewModel model)
    {
        AttachToken();
        var success = await _apiClient.CreateEventAsync(model);
        if (!success)
        {
            ModelState.AddModelError("", "Could not create the event. Check your details and try again.");
            return View(model);
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        AttachToken();
        var ev = await _apiClient.GetEventAsync(id);
        if (ev is null) return NotFound();

        // GetEventAsync hits the public GET /api/events/{id} endpoint, which returns
        // ANY event regardless of owner (public visitors need that for the details page).
        // So ownership has to be checked here explicitly before showing the edit form -
        // otherwise someone could browse another organizer's event data by guessing an id.
        if (ev.OrganizerId != CurrentUserId)
        {
            TempData["Message"] = "You can only edit events you created.";
            return RedirectToAction(nameof(Index));
        }

        var model = new CreateEventViewModel
        {
            Title = ev.Title,
            Description = ev.Description,
            EventType = ev.EventType,
            EventDate = ev.EventDate,
            Location = ev.Location,
            Capacity = ev.Capacity
        };
        ViewBag.EventId = id;
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, CreateEventViewModel model)
    {
        AttachToken();
        // Even with the GET-side check above, the API's EventService.UpdateAsync is the
        // real enforcement point - it re-checks ownership server-side on every save,
        // so a forged POST request can never update someone else's event either.
        var success = await _apiClient.UpdateEventAsync(id, model);
        if (!success)
        {
            ModelState.AddModelError("", "Could not update the event. You can only edit events you created.");
            ViewBag.EventId = id;
            return View(model);
        }
        TempData["Message"] = "Event updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Registrations(int id)
    {
        AttachToken();
        var ev = await _apiClient.GetEventAsync(id);
        if (ev is null) return NotFound();

        if (ev.OrganizerId != CurrentUserId)
        {
            TempData["Message"] = "You can only view registrations for events you created.";
            return RedirectToAction(nameof(Index));
        }

        var registrations = await _apiClient.GetRegistrationsForEventAsync(id);
        if (registrations is null)
        {
            TempData["Message"] = "You can only view registrations for events you created.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.EventTitle = ev.Title;
        ViewBag.EventId = id;
        return View(registrations);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        AttachToken();
        var deleted = await _apiClient.DeleteEventAsync(id);
        TempData["Message"] = deleted
            ? "Event deleted."
            : "Could not delete that event - you can only delete events you created.";
        return RedirectToAction(nameof(Index));
    }
}
