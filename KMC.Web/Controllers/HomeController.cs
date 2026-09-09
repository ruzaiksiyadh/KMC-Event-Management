using KMC.Web.Models;
using KMC.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace KMC.Web.Controllers;

public class HomeController : Controller
{
    private readonly ApiClient _apiClient;

    public HomeController(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<IActionResult> Index()
    {
        List<EventViewModel> events;
        try
        {
            // Same public search endpoint the Events page uses - no filters means "everything".
            events = await _apiClient.SearchEventsAsync(null, null, null, null);
        }
        catch
        {
            // API might not be running yet - fail gracefully instead of crashing the homepage.
            events = new List<EventViewModel>();
        }

        var upcoming = events.Where(e => e.EventDate >= DateTime.Today).OrderBy(e => e.EventDate).ToList();

        var vm = new HomeViewModel
        {
            FeaturedEvents = (upcoming.Any() ? upcoming : events.OrderBy(e => e.EventDate).ToList()).Take(6).ToList(),
            TotalEvents = events.Count,
            TotalParticipants = events.Sum(e => e.RegisteredCount),
            TotalOrganizers = events.Select(e => e.OrganizerName).Where(n => !string.IsNullOrWhiteSpace(n)).Distinct().Count(),
            TotalCategories = events.Select(e => e.EventType).Where(t => !string.IsNullOrWhiteSpace(t)).Distinct().Count()
        };

        return View(vm);
    }

    public IActionResult Error() => View();
}
