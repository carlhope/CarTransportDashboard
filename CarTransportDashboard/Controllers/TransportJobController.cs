using CarTransportDashboard.Models;
using Microsoft.AspNetCore.Mvc;
using CarTransportDashboard.Models.Dtos.TransportJob;
using CarTransportDashboard.Services.Interfaces;

namespace CarTransportDashboard.Controllers;



[ApiController]
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
        var createdJob = await _jobService.CreateJobAsync(dto);
        return CreatedAtAction(nameof(GetJob), new { id = createdJob.Id }, createdJob);
    }

    // PUT: api/transportjobs/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateJob(Guid id, [FromBody] TransportJobWriteDto dto)
    {
        var existing = await _jobService.GetJobAsync(id);
        if (existing == null)
            return NotFound();

        await _jobService.UpdateJobAsync(id, dto);
        return NoContent();
    }

    // PATCH: api/transportjobs/{id}/status
    [HttpPatch("{id}/status")]
    public async Task<ActionResult> UpdateJobStatus(Guid id, [FromBody] JobStatus status)
    {
        var existing = await _jobService.GetJobAsync(id);
        if (existing == null)
            return NotFound();

        await _jobService.UpdateJobStatusAsync(id, status);
        return NoContent();
    }

    // POST: api/transportjobs/{id}/accept
    [HttpPost("{id}/accept")]
    public async Task<ActionResult> AcceptJob(Guid id, [FromBody] string driverId)
    {
        var existing = await _jobService.GetJobAsync(id);
        if (existing == null)
            return NotFound();

        await _jobService.AcceptJobAsync(id, driverId);
        return NoContent();
    }

    // POST: api/transportjobs/{id}/assign-vehicle
    [HttpPost("{id}/assign-vehicle")]
    public async Task<ActionResult> AssignVehicle(Guid id, [FromBody] Guid vehicleId)
    {
        var existing = await _jobService.GetJobAsync(id);
        if (existing == null)
            return NotFound();

        await _jobService.AssignVehicleToJobAsync(id, vehicleId);
        return NoContent();
    }

    // POST: api/transportjobs/{id}/assign-driver
    [HttpPost("{id}/assign-driver")]
    public async Task<ActionResult> AssignDriver(Guid id, [FromBody] string driverId)
    {
        var existing = await _jobService.GetJobAsync(id);
        if (existing == null)
            return NotFound();

        await _jobService.AssignDriverToJobAsync(id, driverId);
        return NoContent();
    }
}
