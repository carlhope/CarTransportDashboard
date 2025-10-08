using CarTransportDashboard.Models;
using CarTransportDashboard.Models.Dtos.TransportJob;
using CarTransportDashboard.Models.Users;
using CarTransportDashboard.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarTransportDashboard.Controllers;



[ApiController]
[Authorize] // Require authentication for all endpoints
[Route("api/[controller]")]
public class TransportJobsController : ControllerBase
{
    private readonly ITransportJobService _jobService;

    public TransportJobsController(ITransportJobService jobService)
    {
        _jobService = jobService;
    }

    // GET: api/transportjobs
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransportJobReadDto>>> GetJobs()
    {
        var jobs = await _jobService.GetJobsAsync();
        return Ok(jobs);
    }
    // GET: api/transportjobs/myjobs?status=pending
    [HttpGet("myjobs")]
    public async Task<ActionResult<IEnumerable<TransportJobReadDto>>> GetJobsByUserId([FromQuery]string status)
    {
      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        List<string> users = new List<string>() {userId };
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID claim missing or invalid.");
        var jobs = await _jobService.GetJobsByDriverIdAsync(users, status!);
        return Ok(jobs);
    }

    // GET: api/transportjobs/available
    [HttpGet("available")]
    public async Task<ActionResult<IEnumerable<TransportJobReadDto>>> GetAvailableJobs()
    {
        var jobs = await _jobService.GetAvailableJobsAsync();
        return Ok(jobs);
    }

    // GET: api/transportjobs/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<TransportJobReadDto>> GetJob(Guid id)
    {
        var job = await _jobService.GetJobAsync(id);
        if (job == null)
            return NotFound();

        return Ok(job);
    }

    // POST: api/transportjobs
    [HttpPost]
    public async Task<ActionResult<TransportJobReadDto>> CreateJob([FromBody] TransportJobWriteDto dto)
    {
        var result = await _jobService.CreateJobAsync(dto);

        if (!result.Success || result.Data is null)
            return BadRequest(result.Message);

        return CreatedAtAction(nameof(GetJob), new { id = result.Data.Id }, result.Data);
    }

    // PUT: api/transportjobs/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateJob(Guid id, [FromBody] TransportJobWriteDto dto)
    {
        var result = await _jobService.UpdateJobAsync(id, dto);

        if (!result.Success)
            return NotFound(result.Message);

        return Ok(result.Data);
    }

    // PATCH: api/transportjobs/{id}/status
    [HttpPatch("{id}/status")]
    public async Task<ActionResult> UpdateJobStatus(Guid id, [FromBody] JobStatus status)
    {
        var result = await _jobService.UpdateJobStatusAsync(id, status);

        if (!result.Success)
            return NotFound(result.Message);

        return Ok(result.Data);
    }

    // POST: api/transportjobs/{id}/accept
    [HttpPost("{id}/accept")]
    [Authorize(Roles = "Driver")] // Optional: restrict to drivers
    public async Task<ActionResult> AcceptJob(Guid id)
    {
        var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(driverId))
            return Unauthorized("User identity not found.");

        var result = await _jobService.AcceptJobAsync(id, driverId);

        if (!result.Success)
            return NotFound(result.Message);

        return Ok(result.Data);
    }

    // POST: api/transportjobs/{id}/assign-vehicle
    [Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.Dispatcher)]// restrict to admins/dispatchers
    [HttpPost("{id}/assign-vehicle")]
    public async Task<ActionResult> AssignVehicle(Guid id, [FromBody] Guid vehicleId)
    {
        var result = await _jobService.AssignVehicleToJobAsync(id, vehicleId);

        if (!result.Success)
            return NotFound(result.Message);

        return Ok(result.Data);
    }

    // POST: api/transportjobs/{id}/assign-driver
    [HttpPost("{id}/assign-driver")]
    [Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.Dispatcher)] // restrict to admins/dispatchers
    public async Task<ActionResult> AssignDriver(Guid id, [FromBody] string driverId)
    {
        var result = await _jobService.AssignDriverToJobAsync(id, driverId);

        if (!result.Success)
            return NotFound(result.Message);
        
        return Ok(result.Data);
    }
}
