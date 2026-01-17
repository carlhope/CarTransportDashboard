using CarTransportDashboard.Models.Dtos.Vehicle;
using CarTransportDashboard.Models;

namespace CarTransportDashboard.Mappers;
public class VehicleMapper
{
    public static VehicleReadDto ToDto(Vehicle vehicle)
    {
        return new VehicleReadDto
        {
            Id = vehicle.Id,
            Make = vehicle.Make,
            Model = vehicle.Model,
            RegistrationNumber = vehicle.RegistrationNumber,
            FuelType = vehicle.FuelType,
        };
    }
    public static VehicleWriteDto ToWriteDto(Vehicle vehicle)
    {
        return new VehicleWriteDto
        {
            Id = vehicle.Id,
            Make = vehicle.Make,
            Model = vehicle.Model,
            RegistrationNumber = vehicle.RegistrationNumber,
            FuelType = vehicle.FuelType,
        };
    }

    public static Vehicle ToModel(VehicleWriteDto dto)
    {
        return new Vehicle
        {
            Id = dto.Id,
            Make = dto.Make,
            Model = dto.Model,
            RegistrationNumber = dto.RegistrationNumber,
            FuelType = dto.FuelType,
        };
    }
    public static void UpdateModel(Vehicle vehicle, VehicleWriteDto dto)
    {
        vehicle.Make = dto.Make;
        vehicle.Model = dto.Model;
        vehicle.RegistrationNumber = dto.RegistrationNumber;
        vehicle.FuelType = dto.FuelType;
    }
}