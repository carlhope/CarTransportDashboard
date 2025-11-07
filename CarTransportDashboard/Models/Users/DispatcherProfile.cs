using CarTransportDashboard.Context;
using System.ComponentModel.DataAnnotations;

namespace CarTransportDashboard.Models.Users
{
    public class DispatcherProfile : IHasUserId
    {
        [Key]
        public required string UserId { get; set; }

        private readonly List<DriverProfile> _managedDrivers = new();
        public IReadOnlyCollection<DriverProfile> ManagedDrivers => _managedDrivers.AsReadOnly();//provides read-only access to the list. Cannot Add/Remove directly.

        public void AddDriver(DriverProfile driver)
        {
            if (_managedDrivers.Any(d => d.UserId == driver.UserId))
                throw new InvalidOperationException("Driver is already managed by this dispatcher.");

            _managedDrivers.Add(driver);
        }

        public void RemoveDriver(string driverUserId)
        {
            var driver = _managedDrivers.FirstOrDefault(d => d.UserId == driverUserId);
            if (driver == null)
                throw new InvalidOperationException("Driver not found in managed list.");
            driver.UnassignDispatcher();
            _managedDrivers.Remove(driver);
        }

        public bool IsManagingDriver(string driverUserId) =>
            _managedDrivers.Any(d => d.UserId == driverUserId);

        public void OfferJobToDriver(TransportJob job, ApplicationUser driver)
        {
            if (!IsManagingDriver(driver.Id))
                throw new InvalidOperationException("Dispatcher does not manage this driver.");

            job.AssignDriver(driver);
        }
    }
}
