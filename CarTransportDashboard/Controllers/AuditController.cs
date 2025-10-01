
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CarTransportDashboard.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AuditController : ControllerBase
{
    [HttpGet("logs")]
    public IActionResult GetLogs()
    {
        // TODO: Return audit logs
        return Ok(/* logs */);
    }

    [HttpGet("changes")]
    public IActionResult GetEntityChanges(string entity, string id)
    {
        // TODO: Return change history for entity
        return Ok(/* changes */);
    }
}