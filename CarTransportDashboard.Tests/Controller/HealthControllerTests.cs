using CarTransportDashboard.Controllers;
using CarTransportDashboard.Models.Dtos.Health;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CarTransportDashboard.Tests.Controller
{
  

    public class HealthControllerTests
    {
        [Fact]
        public void Ping_ReturnsOkWithPong()
        {
            // Arrange
            var controller = new HealthController();

            // Act
            var result = controller.Ping();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("pong", okResult.Value);
        }

        [Fact]
        public void Status_ReturnsOkWithHealthPayload()
        {
            // Arrange
            var controller = new HealthController();

            // Act
            var result = controller.Status();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<HealthStatusDto>(okResult.Value);

            Assert.Equal("Healthy", payload.Status);
            Assert.IsType<DateTime>(payload.Timestamp);
        }

        //TODO: Currently only covers happy paths. Add more tests if more complex logic is added.

    }
}
