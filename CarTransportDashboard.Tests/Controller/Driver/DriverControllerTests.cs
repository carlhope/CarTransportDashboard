using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarTransportDashboard.Tests.Controller.Driver
{
    using Xunit;
    using Moq;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using CarTransportDashboard.Controllers;
    using CarTransportDashboard.Services.Interfaces;
    using CarTransportDashboard.Models.Dtos.TransportJob;

    public class DriverControllerTests
    {
        [Fact]
        public async Task GetAssignedJobs_WithValidUserId_ReturnsOkWithJobs()
        {
            // Arrange
            var userId = "driver-123";
            var mockService = new Mock<IDriverService>();
            var expectedJobs = new List<TransportJobReadDto>
        {
            new() { Id = Guid.NewGuid(), Description = "Deliver vehicle to depot" }
        };

            mockService
                .Setup(s => s.GetAssignedJobsAsync(userId))
                .ReturnsAsync(expectedJobs);

            var controller = new DriverController(mockService.Object);

            // Simulate authenticated user with NameIdentifier claim
            var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            // Act
            var result = await controller.GetAssignedJobs();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedJobs, okResult.Value);
        }
    }
}
