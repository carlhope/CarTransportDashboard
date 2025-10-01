using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CarTransportDashboard.Models.Dtos.Vehicle;
using CarTransportDashboard.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using CarTransportDashboard.Models.Users;

namespace CarTransportDashboard.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Require authentication for all endpoints
public class VehicleController : ControllerBase
{
    private readonly IVehicleService _vehicleService;

    public VehicleController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    // GET: api/vehicles
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VehicleReadDto>>> GetVehicles()
    {
        var vehicles = await _vehicleService.GetVehiclesAsync();
        return Ok(vehicles);
    }

    // GET: api/vehicles/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<VehicleReadDto>> GetVehicle(Guid id)
    {
        var vehicle = await _vehicleService.GetVehicleAsync(id);
        if (vehicle == null)
            return NotFound();

        return Ok(vehicle);
    }
    //GET: api/vehicle/registration/{registrationNumber}
    [HttpGet("registration/{registrationNumber}")]
    public async Task<ActionResult<VehicleReadDto>> GetVehicleByRegistrationNumber(string registrationNumber)
    {
        if (string.IsNullOrWhiteSpace(registrationNumber))
            return BadRequest("Registration number must be provided.");
        var vehicle = await _vehicleService.GetVehicleByRegistrationNumberAsync(registrationNumber);
        if (vehicle == null)
            return NotFound();
        return Ok(vehicle);
    }

    // POST: api/vehicles
    [HttpPost]
    [Authorize(Roles = RoleConstants.Admin+","+ RoleConstants.Dispatcher )] // Only Admins can create vehicles
    public async Task<ActionResult> CreateVehicle([FromBody] VehicleWriteDto dto)
    {
        var result = await _vehicleService.CreateVehicleAsync(dto);

        if (!result.Success || result.Data is null)
            return BadRequest(result.Message);

        return CreatedAtAction(nameof(GetVehicle), new { id = result.Data.Id }, result.Data);
    }

    // PUT: api/vehicles/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.Dispatcher)]
    public async Task<ActionResult> UpdateVehicle(Guid id, [FromBody] VehicleWriteDto dto)
    {
        var result = await _vehicleService.UpdateVehicleAsync(id, dto);

        if (!result.Success)
            return NotFound(result.Message);

        return NoContent();
    }

    // DELETE: api/vehicles/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.Dispatcher)]
    public async Task<ActionResult> DeleteVehicle(Guid id)
    {
        var result = await _vehicleService.DeleteVehicleAsync(id);

        if (!result.Success)
            return NotFound(result.Message);

        return NoContent();
    }
}
