using CarTransportDashboard.Repository.Interfaces;
using CarTransportDashboard.Models;
using CarTransportDashboard.Context;
using Microsoft.EntityFrameworkCore;
namespace CarTransportDashboard.Repository{
    public class TransportJobRepository : ITransportJobRepository
{
    private readonly ApplicationDbContext _context;

    public TransportJobRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TransportJob?> GetByIdAsync(Guid id) =>
        await _context.TransportJobs.FindAsync(id);

    public async Task<IEnumerable<TransportJob>> GetAllAsync() =>
        await _context.TransportJobs.ToListAsync();

    public async Task<IEnumerable<TransportJob>> GetAvailableJobsAsync() =>
        await _context.TransportJobs
            .Where(j => j.AssignedVehicleId != null && j.AssignedDriverId == null)
            .ToListAsync();

    public async Task AddAsync(TransportJob job)
    {
        _context.TransportJobs.Add(job);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TransportJob job)
    {
        _context.TransportJobs.Update(job);
        await _context.SaveChangesAsync();
    }

    public async Task AssignVehicleAsync(Guid jobId, Guid vehicleId)
    {
        var job = await GetByIdAsync(jobId);
        if (job != null)
        {
            job.AssignedVehicleId = vehicleId;
            await UpdateAsync(job);
        }
    }

    public async Task AssignDriverAsync(Guid jobId, string driverId)
    {
        var job = await GetByIdAsync(jobId);
        if (job != null)
        {
            job.AssignedDriverId = driverId;
            await UpdateAsync(job);
        }
    }
}

}