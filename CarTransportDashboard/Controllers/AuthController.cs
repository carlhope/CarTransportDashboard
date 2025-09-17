using CarTransportDashboard.Models.Dtos;
using CarTransportDashboard.Models.Dtos.Auth;
using CarTransportDashboard.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CarTransportDashboard.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto dto)
    {
        var user = await _authService.RegisterAsync(dto);
        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto dto)
    {
        var user = await _authService.LoginAsync(dto.Email, dto.Password);
        if (user == null) return Unauthorized();

        Response.Cookies.Append("refreshToken", user.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // set to true in production
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });

        user.RefreshToken = null; // Don't send to frontend
        return Ok(user);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<UserDto>> Refresh()
    {
        #if !DEBUG
        var origin = Request.Headers["Origin"].ToString();
        if (string.IsNullOrEmpty(origin) || !origin.Equals("http://localhost:4200", StringComparison.OrdinalIgnoreCase))
        {
            return Unauthorized();
        }
        #endif
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken)) return Unauthorized();

        var user = await _authService.RefreshTokenAsync(refreshToken);
        if (user == null) return Unauthorized();

        Response.Cookies.Append("refreshToken", user.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None, // needs to be accessible from angular frontend
            Expires = DateTime.UtcNow.AddDays(7),
            IsEssential = true,
            Path = "/api/auth"
        });

        user.RefreshToken = null;
        return Ok(user);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("refreshToken");
        return NoContent();
    }
}