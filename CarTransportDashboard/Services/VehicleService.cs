using CarTransportDashboard.Models;
using CarTransportDashboard.Models.Dtos.Vehicle;
using CarTransportDashboard.Repository.Interfaces;
using CarTransportDashboard.Services.Interfaces;
using CarTransportDashboard.Mappers;
namespace CarTransportDashboard.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        public VehicleService(IVehicleRepository vehicleRepository)
        {
         _vehicleRepository = vehicleRepository;   
        }

        public async Task<VehicleReadDto?> GetVehicleAsync(Guid id)
        {
            Vehicle? vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle == null) return null;
            VehicleReadDto vehicleDto = VehicleMapper.ToDto(vehicle);
            return vehicleDto;
        }
        public async Task<Vehicle?> GetVehicleByRegistrationNumberAsync(string registrationNumber)
        {
            registrationNumber = NormalizeRegistration(registrationNumber);
            return await _vehicleRepository.GetByRegistrationNumberAsync(registrationNumber);
        }

        public async Task<IEnumerable<VehicleReadDto>> GetVehiclesAsync()
        {
            IEnumerable<Vehicle> vehicles = await _vehicleRepository.GetAllAsync();
            return vehicles.Select(VehicleMapper.ToDto);

        }
        public async Task<OperationResult<VehicleReadDto>> CreateVehicleAsync(VehicleWriteDto dto)
        {
            var vehicle = VehicleMapper.ToModel(dto);
            vehicle.RegistrationNumber = NormalizeRegistration(vehicle.RegistrationNumber);

            var result = await _vehicleRepository.AddAsync(vehicle);

            if (!result.Success || result.Data is null)
                return OperationResult<VehicleReadDto>.CreateFailure(result.Message);

            var readDto = VehicleMapper.ToDto(result.Data);
            return OperationResult<VehicleReadDto>.CreateSuccess(readDto, "Vehicle created successfully.");
        }

        public async Task<OperationResult<VehicleReadDto>> DeleteVehicleAsync(Guid id)
        {
            var result = await _vehicleRepository.DeleteAsync(id);

            if (!result.Success || result.Data is null)
                return OperationResult<VehicleReadDto>.CreateFailure(result.Message);

            var readDto = VehicleMapper.ToDto(result.Data);
            return OperationResult<VehicleReadDto>.CreateSuccess(readDto, "Vehicle deleted successfully.");
        }

        public async Task<OperationResult<VehicleReadDto>> UpdateVehicleAsync(Guid id, VehicleWriteDto dto)
        {
            var existingVehicle = await _vehicleRepository.GetByIdAsync(id);
            if (existingVehicle is null)
                return OperationResult<VehicleReadDto>.CreateFailure($"Vehicle with ID {id} not found.");

            VehicleMapper.UpdateModel(existingVehicle, dto);
            existingVehicle.RegistrationNumber = NormalizeRegistration(existingVehicle.RegistrationNumber);
            var result = await _vehicleRepository.UpdateAsync(existingVehicle);

            if (!result.Success || result.Data is null)
                return OperationResult<VehicleReadDto>.CreateFailure(result.Message);

            var readDto = VehicleMapper.ToDto(result.Data);
            return OperationResult<VehicleReadDto>.CreateSuccess(readDto, "Vehicle updated successfully.");
        }
        private static string NormalizeRegistration(string input) //ensures consistent format
        {
            return new string(input
                .Where(char.IsLetterOrDigit)
                .ToArray())
                .ToUpperInvariant();
        }


    }
}