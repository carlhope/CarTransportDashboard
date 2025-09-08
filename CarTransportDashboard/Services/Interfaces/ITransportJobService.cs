using CarTransportDashboard.Models;
namespace CarTransportDashboard.Services.Interfaces
{
    public interface ITransportJobService
    {
        Task<TransportJob?> GetJobAsync(Guid id);
        Task<IEnumerable<TransportJob>> GetJobsAsync();
        Task<IEnumerable<TransportJob>> GetAvailableJobsAsync();
        Task AcceptJobAsync(Guid jobId, string driverId);
        Task UpdateJobStatusAsync(Guid jobId, JobStatus status);
        Task AssignVehicleToJobAsync(Guid jobId, Guid vehicleId);
        Task AssignDriverToJobAsync(Guid jobId, Guid driverId);
    }

}