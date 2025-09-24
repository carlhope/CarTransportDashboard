using CarTransportDashboard.Models;
using CarTransportDashboard.Models.Dtos.Vehicle;
namespace CarTransportDashboard.Services.Interfaces
{
    public interface IVehicleService
    {
        Task<VehicleReadDto?> GetVehicleAsync(Guid id);
        Task<IEnumerable<VehicleReadDto>> GetVehiclesAsync();
        Task<OperationResult<VehicleReadDto>> UpdateVehicleAsync(Guid id, VehicleWriteDto dto);
        Task<OperationResult<VehicleReadDto>> DeleteVehicleAsync(Guid id);
        Task<OperationResult<VehicleReadDto>> CreateVehicleAsync(VehicleWriteDto dto);
        Task<Vehicle?> GetVehicleByRegistrationNumberAsync(string registrationNumber);
    }

}