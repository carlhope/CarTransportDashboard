using CarTransportDashboard.Models;
namespace CarTransportDashboard.Repository.Interfaces
{

    public interface ITransportJobRepository
    {
        Task<TransportJob?> GetByIdAsync(Guid id);
        Task<IEnumerable<TransportJob>> GetAllAsync();
        Task<IEnumerable<TransportJob>> GetAvailableJobsAsync();
        Task AddAsync(TransportJob job);
        Task UpdateAsync(TransportJob job);
        Task AssignVehicleAsync(Guid jobId, Guid vehicleId);
        Task AssignDriverAsync(Guid jobId, string driverId);
    }
}
