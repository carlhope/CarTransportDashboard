using CarTransportDashboard.Models.Users;
using CarTransportDashboard.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace CarTransportDashboard.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DriverController : ControllerBase
{
    private readonly IDriverService _driverService;

    public DriverController(IDriverService driverService)
    {
        _driverService = driverService;
        
    }



[HttpGet("jobs")]
[Authorize(Roles = RoleConstants.Driver)]
public async Task<IActionResult> GetAssignedJobs()
{
        //possibly redundant as method now provided in TransportJobController. Retain for now for clarity
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID claim missing or invalid.");
        var jobs = await _driverService.GetAssignedJobsAsync(userId);
    return Ok(jobs);
}

}