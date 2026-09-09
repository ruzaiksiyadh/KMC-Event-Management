using KMC.Api.DTOs;
using KMC.Api.Models;
using KMC.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KMC.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;

    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    // Event organizers register here (public site visitors don't need an account).
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterOrganizerDto dto)
    {
        var user = new ApplicationUser { UserName = dto.Email, Email = dto.Email, DisplayName = dto.DisplayName };
        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        var (token, expires) = _tokenService.CreateToken(user);
        return Ok(new AuthResponseDto(token, expires, user.DisplayName, user.Id));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null) return Unauthorized("Invalid email or password.");

        var check = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!check.Succeeded) return Unauthorized("Invalid email or password.");

        var (token, expires) = _tokenService.CreateToken(user);
        return Ok(new AuthResponseDto(token, expires, user.DisplayName, user.Id));
    }
}
