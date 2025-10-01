using Microsoft.AspNetCore.Mvc;


namespace CarTransportDashboard.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Ok("pong");

    [HttpGet("status")]
    public IActionResult Status()
    {
        // TODO: Add more detailed health checks as needed
        return Ok(new { status = "Healthy", timestamp = DateTime.UtcNow });
    }
}