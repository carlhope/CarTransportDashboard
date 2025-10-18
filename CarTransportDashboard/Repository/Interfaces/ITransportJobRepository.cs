using CarTransportDashboard.Models;
using CarTransportDashboard.Models.Dtos.TransportJob;
using CarTransportDashboard.Models.Users;
namespace CarTransportDashboard.Repository.Interfaces
{

    public interface ITransportJobRepository
    {
        Task<TransportJob?> GetByIdAsync(Guid id);
        Task<IEnumerable<TransportJob>> GetAllAsync();
        Task<IEnumerable<TransportJob>> GetAvailableJobsAsync();
        Task<IEnumerable<TransportJob>> GetAllByDriverIdsAsync(IEnumerable<string> driverIds, string? status, DateTime? startDate = null);
        Task<OperationResult<TransportJob>> AddAsync(TransportJob job);
        //Task<OperationResult<TransportJob>> AssignVehicleAsync(Guid jobId, Guid vehicleId);
        Task<OperationResult<TransportJob>> UpdateAsync(TransportJob job);
        
    }
}
