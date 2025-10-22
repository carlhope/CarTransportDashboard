using CarTransportDashboard.Models.Dtos;
using CarTransportDashboard.Models.Dtos.Auth;
using CarTransportDashboard.Services;
using CarTransportDashboard.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CarTransportDashboard.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IWebHostEnvironment _env;
    private readonly ICsrfValidator _csrfValidator;

    public AuthController(IAuthService authService, IWebHostEnvironment env, ICsrfValidator csrfValidator)
    {
        _authService = authService;
        _env = env;
        _csrfValidator = csrfValidator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto dto)
    {
        var user = await _authService.RegisterAsync(dto);
        if (user == null) return BadRequest("Registration failed");

        // Set refresh token cookie
        Response.Cookies.Append("refreshToken", user.RefreshToken, GetRefreshCookieOptions());
        Response.Cookies.Append("X-CSRF-Token", user.CsrfToken, new CookieOptions
        {
            HttpOnly = false,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/"
        });


        user.RefreshToken = "0";
        return Ok(user);
    }


    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto dto)
    {
        var user = await _authService.LoginAsync(dto.Email, dto.Password);
        if (user == null) return Unauthorized();

        Response.Cookies.Append("refreshToken", user.RefreshToken, GetRefreshCookieOptions());
        Response.Cookies.Append("X-CSRF-Token", user.CsrfToken, new CookieOptions
        {
            HttpOnly = false, // must be readable by JS to send in header
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/"
        });


        user.RefreshToken = "0"; // Don't send to frontend
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
        if (!_csrfValidator.IsValid(Request))
            return Unauthorized();


        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken)) return Unauthorized();

        var user = await _authService.RefreshTokenAsync(refreshToken);
        if (user == null) return Unauthorized();

        Response.Cookies.Append("refreshToken", user.RefreshToken, GetRefreshCookieOptions());
        user.RefreshToken = "0";
        return Ok(user);
    }

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        if (!_csrfValidator.IsValid(Request))
            return Unauthorized();


        Response.Cookies.Delete("refreshToken", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddDays(7),
            IsEssential = true,
            Path = "/"
        });

        return NoContent();
    }

    private CookieOptions GetRefreshCookieOptions()
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddDays(7),
            IsEssential = true,
            Path = "/"
        };
    }
}