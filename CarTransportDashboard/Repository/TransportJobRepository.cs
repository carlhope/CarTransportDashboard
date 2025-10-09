using CarTransportDashboard.Context;
using CarTransportDashboard.Models;
using CarTransportDashboard.Repository.Interfaces;
using CarTransportDashboard.Services;
using Microsoft.EntityFrameworkCore;
namespace CarTransportDashboard.Repository{
    public class TransportJobRepository : ITransportJobRepository
{
    private readonly ApplicationDbContext _context;
        private readonly ILogger<TransportJobRepository> _logger;


        public TransportJobRepository(ApplicationDbContext context, ILogger<TransportJobRepository> logger)
    {
        _context = context;
        _logger = logger;
        }

    public async Task<TransportJob?> GetByIdAsync(Guid id) =>
        await _context.TransportJobs.FindAsync(id);

    public async Task<IEnumerable<TransportJob>> GetAllAsync() =>
        await _context.TransportJobs.ToListAsync();

        public async Task<IEnumerable<TransportJob>> GetAllByDriverIdsAsync(IEnumerable<string> driverIds, string? status)
        {
            var query = _context.TransportJobs.AsQueryable();

            query = query.Where(j => driverIds.Contains(j.AssignedDriverId));

            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<JobStatus>(status, true, out var parsedStatus))
            {
                query = query.Where(j => j.Status == parsedStatus);
            }

            return await query.ToListAsync();
        }


        public async Task<IEnumerable<TransportJob>> GetAvailableJobsAsync() =>
        await _context.TransportJobs
            .Where(j=>j.Status ==JobStatus.Available)
            .ToListAsync();

        public async Task<OperationResult<TransportJob>> AddAsync(TransportJob job)
        {
            try
            {
                _context.TransportJobs.Add(job);
                await _context.SaveChangesAsync();

                return OperationResult<TransportJob>.CreateSuccess(job, "Job created successfully.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update failed while adding TransportJob.");
                return OperationResult<TransportJob>.CreateFailure("Database update failed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while adding TransportJob.");
                return OperationResult<TransportJob>.CreateFailure("An unexpected error occurred.");
            }
        }

        public async Task<OperationResult<TransportJob>> UpdateAsync(TransportJob job)
        {
            try
            {
                job.UpdatedAt = DateTime.UtcNow;
                _context.TransportJobs.Update(job);
                await _context.SaveChangesAsync();
                return OperationResult<TransportJob>.CreateSuccess(job, "Job updated successfully.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update failed while updating TransportJob.");
                return OperationResult<TransportJob>.CreateFailure("Database update failed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating TransportJob.");
                return OperationResult<TransportJob>.CreateFailure("An unexpected error occurred.");
            }
        }

        public async Task<OperationResult<TransportJob>> AssignVehicleAsync(Guid jobId, Guid vehicleId)
        {
            var job = await GetByIdAsync(jobId);
            if (job is null)
                return OperationResult<TransportJob>.CreateFailure("Transport job not found.");
            job.UpdatedAt = DateTime.UtcNow;
            job.AssignedVehicleId = vehicleId;
            return await UpdateAsync(job);
        }

        public async Task<OperationResult<TransportJob>> AssignDriverAsync(Guid jobId, string driverId)
        {
            var job = await GetByIdAsync(jobId);
            if (job is null)
                return OperationResult<TransportJob>.CreateFailure("Transport job not found.");

            job.AssignedDriverId = driverId;
            return await UpdateAsync(job);
        }
    }

}