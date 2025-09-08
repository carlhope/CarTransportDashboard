using CarTransportDashboard.Repository.Interfaces;
using CarTransportDashboard.Models;
using CarTransportDashboard.Context;
using Microsoft.EntityFrameworkCore;

namespace CarTransportDashboard.Repository
{
    public class VehicleRepository : IVehicleRepository
{
    private readonly ApplicationDbContext _context;

    public VehicleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Vehicle?> GetByIdAsync(Guid id) =>
        await _context.Vehicles.FindAsync(id);

    public async Task<IEnumerable<Vehicle>> GetAllAsync() =>
        await _context.Vehicles.ToListAsync();

    public async Task AddAsync(Vehicle vehicle)
    {
        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Vehicle vehicle)
    {
        _context.Vehicles.Update(vehicle);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var vehicle = await GetByIdAsync(id);
        if (vehicle != null)
        {
            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsAssignedToActiveJobAsync(Guid vehicleId) =>
        await _context.TransportJobs.AnyAsync(j =>
            j.AssignedVehicleId == vehicleId && j.Status != JobStatus.Completed);
}

}