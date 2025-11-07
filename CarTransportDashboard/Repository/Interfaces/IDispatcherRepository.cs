using CarTransportDashboard.Models;
using CarTransportDashboard.Models.Users;

namespace CarTransportDashboard.Repository.Interfaces
{
    public interface IDispatcherRepository
    {
        // Dispatcher context
        Task<DispatcherProfile?> GetDispatcherProfileByUserIdAsync(string userId);

        // Driver management
        Task<List<DriverProfile>> GetManagedDriversAsync(string dispatcherUserId);
        Task AddManagedDriverAsync(string dispatcherUserId, string driverUserId);
        Task RemoveManagedDriverAsync(string dispatcherUserId, string driverUserId);

        // Job offering
        Task<bool> OfferJobToDriverAsync(string dispatcherUserId, string driverUserId, Guid jobId);
        Task<List<TransportJob>> GetOfferedJobsForDriverAsync(string driverUserId);
        Task<bool> WithdrawOfferedJobAsync(string dispatcherUserId, string driverUserId, Guid jobId);
    }
}
