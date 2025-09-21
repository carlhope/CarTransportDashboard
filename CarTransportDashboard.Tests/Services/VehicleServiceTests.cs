using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarTransportDashboard.Models;
using CarTransportDashboard.Models.Dtos.Vehicle;
using CarTransportDashboard.Repository.Interfaces;
using CarTransportDashboard.Services;
using Moq;
using Xunit;
namespace CarTransportDashboard.Tests.Services;
public class VehicleServiceTests
{
    private readonly Mock<IVehicleRepository> _vehicleRepoMock = new();

    private VehicleService CreateService() =>
        new VehicleService(_vehicleRepoMock.Object);

    [Fact]
    public async Task CreateVehicleAsync_CallsAddAsyncWithMappedVehicle()
    {
        var dto = new VehicleWriteDto
        {
            Id = Guid.NewGuid(),
            Make = "Ford",
            Model = "Focus",
            RegistrationNumber = "ABC123"
        };

        Vehicle? addedVehicle = null;
        _vehicleRepoMock.Setup(r => r.AddAsync(It.IsAny<Vehicle>()))
       .ReturnsAsync((Vehicle v) =>
       {
           addedVehicle = v;
           return OperationResult<Vehicle>.CreateSuccess(v);
       });

        var service = CreateService();
        var result = await service.CreateVehicleAsync(dto);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(dto.Id, result.Data.Id);

        Assert.NotNull(addedVehicle);
        Assert.Equal(dto.Id, addedVehicle.Id);
        Assert.Equal(dto.Make, addedVehicle.Make);
        Assert.Equal(dto.Model, addedVehicle.Model);
        Assert.Equal(dto.RegistrationNumber, addedVehicle.RegistrationNumber);
        _vehicleRepoMock.Verify(r => r.AddAsync(It.IsAny<Vehicle>()), Times.Once);
    }

    [Fact]
    public async Task DeleteVehicleAsync_CallsDeleteAsync()
    {
        var id = Guid.NewGuid();
        _vehicleRepoMock.Setup(r => r.DeleteAsync(id))
            .ReturnsAsync(OperationResult<Vehicle>.CreateSuccess(new Vehicle { Id = id }));


        var service = CreateService();
        var result = await service.DeleteVehicleAsync(id);

        _vehicleRepoMock.Verify(r => r.DeleteAsync(id), Times.Once);
        Assert.True(result.Success);
        Assert.Equal(id, result.Data.Id);

    }

    [Fact]
    public async Task GetVehicleAsync_ReturnsDto_WhenVehicleExists()
    {
        var id = Guid.NewGuid();
        var vehicle = new Vehicle
        {
            Id = id,
            Make = "Toyota",
            Model = "Corolla",
            RegistrationNumber = "XYZ789"
        };
        _vehicleRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(vehicle);

        var service = CreateService();
        var result = await service.GetVehicleAsync(id);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(vehicle.Make, result.Make);
        Assert.Equal(vehicle.Model, result.Model);
        Assert.Equal(vehicle.RegistrationNumber, result.RegistrationNumber);
    }

    [Fact]
    public async Task GetVehicleAsync_ReturnsNull_WhenVehicleNotFound()
    {
        var id = Guid.NewGuid();
        _vehicleRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Vehicle)null);

        var service = CreateService();
        var result = await service.GetVehicleAsync(id);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetVehiclesAsync_ReturnsDtos()
    {
        var vehicles = new List<Vehicle>
        {
            new Vehicle { Id = Guid.NewGuid(), Make = "Ford", Model = "Fiesta", RegistrationNumber = "AAA111" },
            new Vehicle { Id = Guid.NewGuid(), Make = "Honda", Model = "Civic", RegistrationNumber = "BBB222" }
        };
        _vehicleRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(vehicles);

        var service = CreateService();
        var result = await service.GetVehiclesAsync();

        Assert.Equal(2, result.Count());
        Assert.Contains(result, v => v.Make == "Ford");
        Assert.Contains(result, v => v.Make == "Honda");
    }

    [Fact]
    public async Task UpdateVehicleAsync_UpdatesVehicle_WhenExists()
    {
        var id = Guid.NewGuid();
        var existingVehicle = new Vehicle
        {
            Id = id,
            Make = "Mazda",
            Model = "3",
            RegistrationNumber = "CCC333"
        };
        var dto = new VehicleWriteDto
        {
            Id = id,
            Make = "Mazda",
            Model = "6",
            RegistrationNumber = "CCC333"
        };
        _vehicleRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingVehicle);
        _vehicleRepoMock.Setup(r => r.UpdateAsync(existingVehicle))
            .ReturnsAsync(OperationResult<Vehicle>.CreateSuccess(existingVehicle));


        var service = CreateService();
        var result = await service.UpdateVehicleAsync(id, dto);

        Assert.Equal("6", existingVehicle.Model);
        _vehicleRepoMock.Verify(r => r.UpdateAsync(existingVehicle), Times.Once);
        Assert.True(result.Success);
        Assert.Equal("6", result.Data.Model);

    }

    [Fact]
    public async Task UpdateVehicleAsync_Throws_WhenVehicleNotFound()
    {
        var id = Guid.NewGuid();
        var dto = new VehicleWriteDto
        {
            Id = id,
            Make = "Mazda",
            Model = "6",
            RegistrationNumber = "CCC333"
        };
        _vehicleRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Vehicle)null);

        var service = CreateService();
        var result = await service.UpdateVehicleAsync(id, dto);

        Assert.False(result.Success);
        Assert.Equal($"Vehicle with ID {id} not found.", result.Message);
    }
}