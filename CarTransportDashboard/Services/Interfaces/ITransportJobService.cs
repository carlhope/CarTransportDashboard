using CarTransportDashboard.Models;
using CarTransportDashboard.Models.Dtos.TransportJob;
using CarTransportDashboard.Models.Users;

namespace CarTransportDashboard.Services.Interfaces
{
    public interface ITransportJobService
    {
        Task<TransportJobReadDto?> GetJobAsync(Guid id);
        Task<TransportJob?> GetJobEntityAsync(Guid id);
        Task<IEnumerable<TransportJobReadDto>> GetJobsAsync();
        Task<IEnumerable<TransportJobReadDto>> GetAvailableJobsAsync();
        Task<OperationResult<TransportJobReadDto>> CreateJobAsync(TransportJobWriteDto dto);
        Task<OperationResult<TransportJobReadDto>> AssignDriverToJobAsync(Guid jobId, string driverId);
        Task<OperationResult<TransportJobReadDto>> AssignVehicleToJobAsync(Guid jobId, Guid vehicleId);
        //Task<OperationResult<TransportJobReadDto>> UpdateJobStatusAsync(Guid jobId, JobStatus status);
        Task<OperationResult<TransportJobReadDto>> AcceptJobAsync(Guid jobId, string driverId);
        Task<OperationResult<TransportJobReadDto>> UpdateJobAsync(Guid jobId, TransportJobWriteDto dto);
        Task<IEnumerable<TransportJobReadDto>> GetJobsByDriverIdAsync(List<string> id, string? status, DateTime? startDate = null);
        Task<OperationResult<TransportJobReadDto>> CompleteJobAsync(TransportJob job);
        Task<OperationResult<TransportJobReadDto>> UnassignDriverFromJobAsync(TransportJob job);
        Task<OperationResult<TransportJobReadDto>> CancelJob(TransportJob job);
    }

}