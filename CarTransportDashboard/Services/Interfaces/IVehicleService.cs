using CarTransportDashboard.Models;
namespace CarTransportDashboard.Services.Interfaces
{
    public interface IVehicleService
    {
        Task<Vehicle?> GetVehicleAsync(Guid id);
        Task<IEnumerable<Vehicle>> GetVehiclesAsync();
        Task CreateVehicleAsync(Vehicle vehicle);
        Task UpdateVehicleAsync(Vehicle vehicle);
        Task DeleteVehicleAsync(Guid id);
    }

}