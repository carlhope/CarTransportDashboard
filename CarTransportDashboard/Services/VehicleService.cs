using CarTransportDashboard.Models;
using CarTransportDashboard.Services.Interfaces;
namespace CarTransportDashboard.Services
{
    public class VehicleService : IVehicleService
    {
        public Task CreateVehicleAsync(Vehicle vehicle)
        {
            throw new NotImplementedException();
        }

        public Task DeleteVehicleAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Vehicle?> GetVehicleAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Vehicle>> GetVehiclesAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateVehicleAsync(Vehicle vehicle)
        {
            throw new NotImplementedException();
        }
    }
}