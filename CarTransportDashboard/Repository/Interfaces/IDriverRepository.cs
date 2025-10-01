using CarTransportDashboard.Context;
using CarTransportDashboard.Models;
using CarTransportDashboard.Models.Users;

namespace CarTransportDashboard.Repository.Interfaces
{
    public interface IDriverRepository
    {
        Task<DriverProfile?> GetByIdAsync(string id);
        Task<IEnumerable<TransportJob>> GetAssignedJobsAsync(string driverId);
        Task<bool> IsInDriverRoleAsync(string id);

    }
}
