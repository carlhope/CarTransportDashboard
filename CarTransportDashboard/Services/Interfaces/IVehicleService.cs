using CarTransportDashboard.Models.Dtos.Vehicle;
namespace CarTransportDashboard.Services.Interfaces
{
    public interface IVehicleService
    {
        Task<VehicleReadDto?> GetVehicleAsync(Guid id);
        Task<IEnumerable<VehicleReadDto>> GetVehiclesAsync();
        Task CreateVehicleAsync(VehicleWriteDto dto);
        Task UpdateVehicleAsync(Guid id, VehicleWriteDto dto);
        Task DeleteVehicleAsync(Guid id);
    }

}