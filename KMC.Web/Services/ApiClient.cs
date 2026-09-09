using System.Net.Http.Headers;
using System.Net.Http.Json;
using KMC.Web.Models;

namespace KMC.Web.Services;

// Every call the MVC app makes to the backend goes through here.
// The web app never talks to a database directly - it only ever consumes the API,
// which is the point the assignment wants demonstrated.
public class ApiClient
{
    private readonly HttpClient _http;

    public ApiClient(HttpClient http)
    {
        _http = http;
    }

    public void SetToken(string? token)
    {
        _http.DefaultRequestHeaders.Authorization =
            string.IsNullOrEmpty(token) ? null : new AuthenticationHeaderValue("Bearer", token);
    }

    // ---- Public browsing ----
    public async Task<List<EventViewModel>> SearchEventsAsync(string? keyword, string? eventType, DateTime? from, DateTime? to)
    {
        var query = new List<string>();
        if (!string.IsNullOrWhiteSpace(keyword)) query.Add($"keyword={Uri.EscapeDataString(keyword)}");
        if (!string.IsNullOrWhiteSpace(eventType)) query.Add($"eventType={Uri.EscapeDataString(eventType)}");
        if (from.HasValue) query.Add($"fromDate={from:yyyy-MM-dd}");
        if (to.HasValue) query.Add($"toDate={to:yyyy-MM-dd}");
        var qs = query.Count > 0 ? "?" + string.Join("&", query) : "";

        var result = await _http.GetFromJsonAsync<List<EventViewModel>>($"api/events{qs}");
        return result ?? new();
    }

    public async Task<EventViewModel?> GetEventAsync(int id) =>
        await _http.GetFromJsonAsync<EventViewModel>($"api/events/{id}");

    public async Task<bool> RegisterParticipantAsync(int eventId, RegisterParticipantViewModel dto)
    {
        var response = await _http.PostAsJsonAsync($"api/events/{eventId}/registrations",
            new { dto.ParticipantName, dto.ParticipantEmail });
        return response.IsSuccessStatusCode;
    }

    // ---- Organizer dashboard (requires token set via SetToken) ----
    public async Task<List<EventViewModel>> GetMyEventsAsync()
    {
        var result = await _http.GetFromJsonAsync<List<EventViewModel>>("api/events/mine");
        return result ?? new();
    }

    public async Task<bool> CreateEventAsync(CreateEventViewModel dto)
    {
        var response = await _http.PostAsJsonAsync("api/events", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateEventAsync(int id, CreateEventViewModel dto)
    {
        var response = await _http.PutAsJsonAsync($"api/events/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteEventAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/events/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<List<RegistrationViewModel>?> GetRegistrationsForEventAsync(int eventId)
    {
        var response = await _http.GetAsync($"api/events/{eventId}/registrations");
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<List<RegistrationViewModel>>() ?? new();
    }

    // ---- Auth ----
    public async Task<(bool success, string? token, string? displayName, string? userId, string? error)> LoginAsync(string email, string password)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", new { email, password });
        if (!response.IsSuccessStatusCode)
            return (false, null, null, null, await response.Content.ReadAsStringAsync());

        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return (true, result?.Token, result?.DisplayName, result?.UserId, null);
    }

    public async Task<(bool success, string? token, string? displayName, string? userId, string? error)> RegisterOrganizerAsync(string displayName, string email, string password)
    {
        var response = await _http.PostAsJsonAsync("api/auth/register", new { displayName, email, password });
        if (!response.IsSuccessStatusCode)
            return (false, null, null, null, await response.Content.ReadAsStringAsync());

        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return (true, result?.Token, result?.DisplayName, result?.UserId, null);
    }

    private record AuthResponse(string Token, DateTime ExpiresAt, string DisplayName, string UserId);
}
