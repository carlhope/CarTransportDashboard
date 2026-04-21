using CarTransportDashboard.Context;
using CarTransportDashboard.Helpers;
using CarTransportDashboard.Helpers.Interfaces;
using CarTransportDashboard.Models.Dtos.Auth;
using CarTransportDashboard.Services.Interfaces;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;


namespace CarTransportDashboard.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IWebHostEnvironment _env;
    private  readonly ICsrfValidator _csrfValidator;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _config;


    public AuthController(IAuthService authService, IWebHostEnvironment env, ICsrfValidator csrfValidator, SignInManager<ApplicationUser> signInManager, IConfiguration config)
    {
        _authService = authService;
        _env = env;
        _csrfValidator = csrfValidator;
        _signInManager = signInManager;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthUserDto>> Register(RegisterDto dto)
    {
        var (csrfToken, user) = await _authService.RegisterAsync(dto);
        if (user == null) return BadRequest("Registration failed");

        // Set cookies
        Response.Cookies.Append("refreshToken", user.RefreshToken, GetRefreshCookieOptions());
        Response.Cookies.Append("X-CSRF-Token", csrfToken, GetCsrfCookieOptions());


        user.RefreshToken = "0";
        return Ok(user);
    }


    [HttpPost("login")]
    public async Task<ActionResult<AuthUserDto>> Login([FromBody] LoginDto dto)
    {
        var (csrfToken, user) = await _authService.LoginAsync(dto.Email, dto.Password);
        if (user == null) return Unauthorized();

        Response.Cookies.Append("refreshToken", user.RefreshToken, GetRefreshCookieOptions());
        Response.Cookies.Append("X-CSRF-Token", csrfToken, GetCsrfCookieOptions());


        user.RefreshToken = "0"; // Don't send to frontend
        return Ok(user);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthUserDto>> Refresh()
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

        var (csrfToken, user) = await _authService.RefreshTokenAsync(refreshToken);
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
            HttpOnly = false,
            Secure = true,
            SameSite = SameSiteMode.None,
            IsEssential = true,
            Path = "/api/auth"
        });

        return NoContent();
    }

    // External authentication endpoints would go here (e.g., Google OAuth)

    [HttpPost("google")]
    [AllowAnonymous]
    public async Task<IActionResult> GoogleLogin([FromBody] IdTokenDto dto, CancellationToken ct)
    {
        var payload = await GoogleJsonWebSignature.ValidateAsync(dto.IdToken);
        if (payload == null || !payload.EmailVerified)
            return Unauthorized("Invalid Google token");

        var (csrfToken, user) = await _authService.FindOrCreateByEmailAsync(payload.Email, payload.GivenName, payload.FamilyName, ct);
       

        Response.Cookies.Append("refreshToken", user.RefreshToken, GetRefreshCookieOptions());
        Response.Cookies.Append("X-CSRF-Token", csrfToken, GetCsrfCookieOptions());

        user.RefreshToken = "0"; // dont send to frontend
        return Ok(user);
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
            Path = "/api/auth"
        };
    }
    private CookieOptions GetCsrfCookieOptions()
    {
        return new CookieOptions
        {
            HttpOnly = false,   // must be accessible by frontend JS
            Secure = true,      // only over HTTPS
            SameSite = SameSiteMode.None, // allow cross-site requests (needed for SPA + API)
            Path = "/",         // available to the whole app
            IsEssential = true  // ensures cookie is not blocked by consent
        };
    }

}
public class IdTokenDto
{
    public string IdToken { get; set; }
}
