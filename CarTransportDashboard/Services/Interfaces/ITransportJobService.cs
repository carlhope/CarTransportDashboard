using CarTransportDashboard.Models;
using CarTransportDashboard.Models.Dtos.TransportJob;
namespace CarTransportDashboard.Services.Interfaces
{
    public interface ITransportJobService
    {
        Task<TransportJobReadDto?> GetJobAsync(Guid id);
        Task<IEnumerable<TransportJobReadDto>> GetJobsAsync();
        Task<IEnumerable<TransportJobReadDto>> GetAvailableJobsAsync();
        Task AcceptJobAsync(Guid jobId, string driverId);
        Task UpdateJobStatusAsync(Guid jobId, JobStatus status);
        Task AssignVehicleToJobAsync(Guid jobId, Guid vehicleId);
        Task AssignDriverToJobAsync(Guid jobId, string driverId);
        // For create/update, add methods using WriteDto if needed
        Task<TransportJobReadDto> CreateJobAsync(TransportJobWriteDto dto);
        Task UpdateJobAsync(Guid jobId, TransportJobWriteDto dto);
    }

}