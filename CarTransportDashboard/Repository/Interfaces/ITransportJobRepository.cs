using CarTransportDashboard.Models;
namespace CarTransportDashboard.Repository.Interfaces
{

    public interface ITransportJobRepository
    {
        Task<TransportJob?> GetByIdAsync(Guid id);
        Task<IEnumerable<TransportJob>> GetAllAsync();
        Task<IEnumerable<TransportJob>> GetAvailableJobsAsync();
        Task<OperationResult<TransportJob>> AddAsync(TransportJob job);
        Task<OperationResult<TransportJob>> AssignDriverAsync(Guid jobId, string driverId);
        Task<OperationResult<TransportJob>> AssignVehicleAsync(Guid jobId, Guid vehicleId);
        Task<OperationResult<TransportJob>> UpdateAsync(TransportJob job);
    }
}
