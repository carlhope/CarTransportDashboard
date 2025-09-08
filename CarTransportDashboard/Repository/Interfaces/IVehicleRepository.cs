using CarTransportDashboard.Models;
namespace CarTransportDashboard.Repository.Interfaces
{
  public interface IVehicleRepository
{
    Task<Vehicle?> GetByIdAsync(Guid id);
    Task<IEnumerable<Vehicle>> GetAllAsync();
    Task AddAsync(Vehicle vehicle);
    Task UpdateAsync(Vehicle vehicle);
    Task DeleteAsync(Guid id);
    Task<bool> IsAssignedToActiveJobAsync(Guid vehicleId);
}


}