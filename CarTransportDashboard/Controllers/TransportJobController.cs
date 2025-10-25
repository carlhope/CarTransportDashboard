using CarTransportDashboard.Models;
using CarTransportDashboard.Models.Dtos.TransportJob;
using CarTransportDashboard.Models.Users;
using CarTransportDashboard.Services;
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
    public async Task<ActionResult<IEnumerable<TransportJobReadDto>>> GetJobsByUserId(
        [FromQuery] string status,
        [FromQuery] DateTime? startDate = null
        )
    {
      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        List<string> users = new List<string>() {userId };
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID claim missing or invalid.");
        var jobs = await _jobService.GetJobsByDriverIdAsync(users, status!, startDate);
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
    public async Task<ActionResult<TransportJobReadDto>> CreateJob([FromBody] TransportJobCreateDto dto)
    {
        var result = await _jobService.CreateJobAsync(dto);

        if (!result.Success || result.Data is null)
            return BadRequest(result.Message);

        return CreatedAtAction(nameof(GetJob), new { id = result.Data.Id }, result.Data);
    }

    // PUT: api/transportjobs/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateJob(Guid id, [FromBody] TransportJobUpdateDto dto)
    {
        var result = await _jobService.UpdateJobAsync(id, dto);

        if (!result.Success)
            return NotFound(result.Message);

        return Ok(result.Data);
    }

    // POST: api/transportjobs/{id}/accept
    //POST is used instead of PATCH as accepting a job may involve more complex processing (e.g. notifications)
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

    // PATCH: api/transportjobs/{id}/assign-vehicle
    [Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.Dispatcher)]// restrict to admins/dispatchers
    [HttpPatch("{id}/assign-vehicle")]
    public async Task<ActionResult> AssignVehicle(Guid id, [FromBody] Guid vehicleId)
    {
        var result = await _jobService.AssignVehicleToJobAsync(id, vehicleId);

        if (!result.Success)
            return NotFound(result.Message);

        return Ok(result.Data);
    }

    // POST: api/transportjobs/{id}/assign-driver
    //POST is used instead of PATCH as assigning a driver may involve more complex processing (e.g. notifications)
    [HttpPost("{id}/assign-driver")]
    [Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.Dispatcher)] // restrict to admins/dispatchers
    public async Task<ActionResult> AssignDriver(Guid id, [FromBody] string driverId)
    {
        var result = await _jobService.AssignDriverToJobAsync(id, driverId);

        if (!result.Success)
            return NotFound(result.Message);
        
        return Ok(result.Data);
    }

    // POST: api/transportjobs/{id}/complete
    //POST used instead of PATCH as this action may involve more complex processing (e.g. triggering payments)
    [HttpPost("{id}/complete")]
    [Authorize(Roles = RoleConstants.Driver)]
    public async Task<IActionResult> CompleteJob(Guid id)
    {
        var job = await _jobService.GetJobEntityAsync(id);
        var result = await _jobService.CompleteJobAsync(job);

        if (!result.Success)
        {
            return BadRequest(new { error = result.Message });
        }

        return Ok(result.Data);
    }

    // POST: api/transportjobs/{id}/cancel
    //POST is used as this action may involve more complex processing (e.g. notifications)
    [HttpPost("{id}/cancel")]
    [Authorize(Roles = RoleConstants.Admin)]
    public async Task<IActionResult> CancelJob(Guid id)
    {
        var job = await _jobService.GetJobEntityAsync(id);
        var result = await _jobService.CancelJob(job);

        if (!result.Success)
        {
            return BadRequest(new { error = result.Message });
        }

        return Ok(result.Data);
    }



    // POST: api/transportjobs/{id}/unassign
    //POST is used instead of PATCH as unassigning may involve more complex processing (e.g. notifications)
    [HttpPost("{id}/unassign")]
    [Authorize]
    public async Task<IActionResult> UnassignJob(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        if (!Enum.TryParse<UserRoles>(userRole, out var parsedRole))
            return Forbid("Invalid user role.");
        //
        var job = await _jobService.GetJobEntityAsync(id);
        OperationResult<TransportJobReadDto> result;
        if (job == null)
            return NotFound("Job not found.");
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User identity not found.");
        if (parsedRole == UserRoles.Driver)
        {
            if (job.AssignedDriverId == userId)
            {
                result = await _jobService.UnassignDriverFromJobAsync(job);
                if (!result.Success)
                    return BadRequest(result.Message);
                Console.WriteLine($"Driver {userId} unassigned themselves from job {id}");
                return Ok(result.Data);
            }
            return Forbid("Drivers can only unassign themselves from jobs.");
        }
        result = await _jobService.UnassignDriverFromJobAsync(job);
        if (!result.Success)
        {
            return BadRequest(result.Message);
        }
        Console.WriteLine($"User {userRole} ({userId}) unassigned driver from job {id}");
        return Ok(result.Data);
    }

}
