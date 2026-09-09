using System.Security.Claims;
using KMC.Web.Models;
using KMC.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace KMC.Web.Controllers;

public class AccountController : Controller
{
    private readonly ApiClient _apiClient;

    public AccountController(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    [HttpGet]
    public IActionResult Login() => View(new LoginViewModel());

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        var (success, token, displayName, userId, error) = await _apiClient.LoginAsync(model.Email, model.Password);
        if (!success)
        {
            model.Error = "Login failed. Check your email and password.";
            return View(model);
        }

        await SignInAsync(token!, displayName!, userId!);
        return RedirectToAction("Index", "Dashboard");
    }

    [HttpGet]
    public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        var (success, token, displayName, userId, error) = await _apiClient.RegisterOrganizerAsync(model.DisplayName, model.Email, model.Password);
        if (!success)
        {
            model.Error = "Could not create account. " + error;
            return View(model);
        }

        await SignInAsync(token!, displayName!, userId!);
        return RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Events");
    }

    private async Task SignInAsync(string token, string displayName, string userId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Name, displayName),
            new("access_token", token) // read back by other controllers to call the API
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
    }
}
