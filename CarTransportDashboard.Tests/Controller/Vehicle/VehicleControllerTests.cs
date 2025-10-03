using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CarTransportDashboard.Controllers;
using CarTransportDashboard.Models.Dtos.Vehicle;
using CarTransportDashboard.Services.Interfaces;
using CarTransportDashboard.Models;
using CarTransportDashboard.Models.Users;
using CarTransportDashboard.Models.Dtos;

namespace CarTransportDashboard.Tests.Controller.Vehicle
{
  

    public class VehicleControllerTests
    {
        private readonly Mock<IVehicleService> _mockService;
        private readonly VehicleController _controller;

        public VehicleControllerTests()
        {
            _mockService = new Mock<IVehicleService>();
            _controller = new VehicleController(_mockService.Object);
        }

        [Fact]
        public async Task GetVehicles_ReturnsOkWithVehicleList()
        {
            var vehicles = new List<VehicleReadDto> { new() { Id = Guid.NewGuid(), RegistrationNumber = "ABC123" } };
            _mockService.Setup(s => s.GetVehiclesAsync()).ReturnsAsync(vehicles);

            var result = await _controller.GetVehicles();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(vehicles, okResult.Value);
        }

        [Fact]
        public async Task GetVehicle_ReturnsOkWithVehicle()
        {
            var id = Guid.NewGuid();
            var vehicle = new VehicleReadDto { Id = id, RegistrationNumber = "XYZ789" };
            _mockService.Setup(s => s.GetVehicleAsync(id)).ReturnsAsync(vehicle);

            var result = await _controller.GetVehicle(id);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(vehicle, okResult.Value);
        }

        [Fact]
        public async Task GetVehicleByRegistrationNumber_ReturnsOkWithVehicle()
        {
            var reg = "DEF456";
            var vehicle = new VehicleReadDto { Id = Guid.NewGuid(), RegistrationNumber = reg };
            _mockService.Setup(s => s.GetVehicleByRegistrationNumberAsync(reg)).ReturnsAsync(vehicle);

            var result = await _controller.GetVehicleByRegistrationNumber(reg);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(vehicle, okResult.Value);
        }
        [Fact]
        public async Task CreateVehicle_ReturnsCreatedAtWithVehicle()
        {
            var dto = new VehicleWriteDto { RegistrationNumber = "NEW123" };
            var createdVehicle = new VehicleReadDto { Id = Guid.NewGuid(), RegistrationNumber = dto.RegistrationNumber };
            var resultWrapper = OperationResult<VehicleReadDto>.CreateSuccess(createdVehicle);

            _mockService
                .Setup(s => s.CreateVehicleAsync(dto))
                .ReturnsAsync(resultWrapper);

            var result = await _controller.CreateVehicle(dto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(createdVehicle, createdResult.Value);
            Assert.Equal(nameof(_controller.GetVehicle), createdResult.ActionName);
        }

        [Fact]
        public async Task UpdateVehicle_ReturnsNoContent()
        {
            var id = Guid.NewGuid();
            var dto = new VehicleWriteDto { RegistrationNumber = "UPD123" };
            var resultWrapper = OperationResult<VehicleReadDto>.CreateSuccess(new VehicleReadDto { Id = id, RegistrationNumber = dto.RegistrationNumber });

            _mockService
                .Setup(s => s.UpdateVehicleAsync(id, dto))
                .ReturnsAsync(resultWrapper);

            var result = await _controller.UpdateVehicle(id, dto);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteVehicle_ReturnsNoContent()
        {
            var id = Guid.NewGuid();
            var resultWrapper = OperationResult<VehicleReadDto>.CreateSuccess(new VehicleReadDto { Id = id, RegistrationNumber = "DEL123" });

            _mockService
                .Setup(s => s.DeleteVehicleAsync(id))
                .ReturnsAsync(resultWrapper);

            var result = await _controller.DeleteVehicle(id);

            Assert.IsType<NoContentResult>(result);
        }

    }
}
