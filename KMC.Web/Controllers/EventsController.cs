using KMC.Web.Models;
using KMC.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace KMC.Web.Controllers;

// Public-facing: no login required. Anyone can browse, search and register for events.
public class EventsController : Controller
{
    private readonly ApiClient _apiClient;

    public EventsController(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? keyword, string? eventType, DateTime? fromDate, DateTime? toDate)
    {
        var results = await _apiClient.SearchEventsAsync(keyword, eventType, fromDate, toDate);
        var vm = new EventSearchViewModel
        {
            Keyword = keyword,
            EventType = eventType,
            FromDate = fromDate,
            ToDate = toDate,
            Results = results
        };
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var ev = await _apiClient.GetEventAsync(id);
        if (ev is null) return NotFound();
        return View(ev);
    }

    [HttpGet]
    public async Task<IActionResult> Register(int id)
    {
        var ev = await _apiClient.GetEventAsync(id);
        if (ev is null) return NotFound();
        return View(new RegisterParticipantViewModel { EventId = id, EventTitle = ev.Title });
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterParticipantViewModel model)
    {
        var success = await _apiClient.RegisterParticipantAsync(model.EventId, model);
        if (!success)
        {
            ModelState.AddModelError("", "Could not register - the event may be full.");
            return View(model);
        }
        TempData["Message"] = "You're registered! See you there.";
        return RedirectToAction(nameof(Details), new { id = model.EventId });
    }
}
