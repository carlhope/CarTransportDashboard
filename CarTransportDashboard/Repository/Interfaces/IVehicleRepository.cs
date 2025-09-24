using CarTransportDashboard.Models;
namespace CarTransportDashboard.Repository.Interfaces
{
  public interface IVehicleRepository
{
    Task<Vehicle?> GetByIdAsync(Guid id);
    Task<IEnumerable<Vehicle>> GetAllAsync();
    Task<bool> IsAssignedToActiveJobAsync(Guid vehicleId);
    Task<OperationResult<Vehicle>> AddAsync(Vehicle vehicle);
    Task<OperationResult<Vehicle>> UpdateAsync(Vehicle vehicle);
    Task<OperationResult<Vehicle>> DeleteAsync(Guid id);
        Task<Vehicle?> GetByRegistrationNumberAsync(string registrationNumber);
    }


}