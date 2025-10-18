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
        Task<OperationResult<TransportJobReadDto>> CreateJobAsync(TransportJobCreateDto dto);
        Task<OperationResult<TransportJobReadDto>> AssignVehicleToJobAsync(Guid jobId, Guid vehicleId);
        Task<OperationResult<TransportJobReadDto>> AcceptJobAsync(Guid jobId, string driverId);
        Task<OperationResult<TransportJobReadDto>> UpdateJobAsync(Guid jobId, TransportJobUpdateDto dto);
        Task<IEnumerable<TransportJobReadDto>> GetJobsByDriverIdAsync(List<string> id, string? status, DateTime? startDate = null);
        Task<OperationResult<TransportJobReadDto>> CompleteJobAsync(TransportJob job);
        Task<OperationResult<TransportJobReadDto>> UnassignDriverFromJobAsync(TransportJob job);
        Task<OperationResult<TransportJobReadDto>> CancelJob(TransportJob job);
        Task<OperationResult<TransportJobReadDto>> AssignDriverToJobAsync(Guid jobId, string driverId);
    }

}