using CarTransportDashboard.Models;
using CarTransportDashboard.Models.Dtos.TransportJob;
namespace CarTransportDashboard.Services.Interfaces
{
    public interface ITransportJobService
    {
        Task<TransportJobReadDto?> GetJobAsync(Guid id);
        Task<IEnumerable<TransportJobReadDto>> GetJobsAsync();
        Task<IEnumerable<TransportJobReadDto>> GetAvailableJobsAsync();
        Task<OperationResult<TransportJobReadDto>> CreateJobAsync(TransportJobWriteDto dto);
        Task<OperationResult<TransportJob>> AssignDriverToJobAsync(Guid jobId, string driverId);
        Task<OperationResult<TransportJob>> AssignVehicleToJobAsync(Guid jobId, Guid vehicleId);
        Task<OperationResult<TransportJob>> UpdateJobStatusAsync(Guid jobId, JobStatus status);
        Task<OperationResult<TransportJob>> AcceptJobAsync(Guid jobId, string driverId);
        Task<OperationResult<TransportJobReadDto>> UpdateJobAsync(Guid jobId, TransportJobWriteDto dto);
    }

}