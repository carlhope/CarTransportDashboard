using CarTransportDashboard.Models;
using CarTransportDashboard.Models.Dtos.Vehicle;
using CarTransportDashboard.Repository.Interfaces;
using CarTransportDashboard.Services.Interfaces;
namespace CarTransportDashboard.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        public VehicleService(IVehicleRepository vehicleRepository)
        {
         _vehicleRepository = vehicleRepository;   
        }
        public async Task CreateVehicleAsync(VehicleWriteDto dto)
        {
            Vehicle vehicle = new Vehicle(dto);
            await _vehicleRepository.AddAsync(vehicle);
        }

        public async Task DeleteVehicleAsync(Guid id)
        {
            await _vehicleRepository.DeleteAsync(id);
        }

        public async Task<VehicleReadDto?> GetVehicleAsync(Guid id)
        {
            Vehicle? vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle == null) return null;
            VehicleReadDto vehicleDto = new VehicleReadDto(vehicle);
            return vehicleDto;
        }

        public async Task<IEnumerable<VehicleReadDto>> GetVehiclesAsync()
        {
            IEnumerable<Vehicle> vehicles = await _vehicleRepository.GetAllAsync();
            return vehicles.Select(v => new VehicleReadDto(v));
            
        }

        public async Task UpdateVehicleAsync(Guid id, VehicleWriteDto dto)
        {
            var existingVehicle = await _vehicleRepository.GetByIdAsync(id);
            if (existingVehicle == null)
                throw new KeyNotFoundException($"Vehicle with ID {id} not found.");
            
            existingVehicle.Make = dto.Make;
            existingVehicle.Model = dto.Model;
            existingVehicle.RegistrationNumber = dto.RegistrationNumber;

            await _vehicleRepository.UpdateAsync(existingVehicle);
        }

    }
}