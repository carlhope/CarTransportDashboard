using CarTransportDashboard.Models;
using CarTransportDashboard.Models.Dtos.Vehicle;
using CarTransportDashboard.Services.Interfaces;
namespace CarTransportDashboard.Services
{
    public class VehicleService : IVehicleService
    {
        public Task<VehicleReadDto> CreateVehicleAsync(VehicleWriteDto dto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteVehicleAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<VehicleReadDto?> GetVehicleAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<VehicleReadDto>> GetVehiclesAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateVehicleAsync(Guid id, VehicleWriteDto dto)
        {
            throw new NotImplementedException();
        }
    }
}