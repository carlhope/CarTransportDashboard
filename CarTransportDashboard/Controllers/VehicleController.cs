using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CarTransportDashboard.Models.Dtos.Vehicle;
using CarTransportDashboard.Services.Interfaces;

namespace CarTransportDashboard.Controllers;

[ApiController]
[Route("api/[controller]")]
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

    // POST: api/vehicles
    [HttpPost]
    public async Task<ActionResult> CreateVehicle([FromBody] VehicleWriteDto dto)
    {
        await _vehicleService.CreateVehicleAsync(dto);
        return CreatedAtAction(nameof(GetVehicle), new { id = dto.Id }, null);
    }

    // PUT: api/vehicles/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateVehicle(Guid id, [FromBody] VehicleWriteDto dto)
    {
        var existing = await _vehicleService.GetVehicleAsync(id);
        if (existing == null)
            return NotFound();

        await _vehicleService.UpdateVehicleAsync(id, dto);
        return NoContent();
    }

    // DELETE: api/vehicles/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteVehicle(Guid id)
    {
        var existing = await _vehicleService.GetVehicleAsync(id);
        if (existing == null)
            return NotFound();

        await _vehicleService.DeleteVehicleAsync(id);
        return NoContent();
    }
}
