using CarTransportDashboard.Controllers;
using CarTransportDashboard.Models;
using CarTransportDashboard.Models.Dtos.TransportJob;
using CarTransportDashboard.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CarTransportDashboard.Tests.Controller.TransportJob
{
    public class TransportJobsControllerTests
    {
        private readonly Mock<ITransportJobService> _mockService;
        private readonly TransportJobsController _controller;

        public TransportJobsControllerTests()
        {
            _mockService = new Mock<ITransportJobService>();
            _controller = new TransportJobsController(_mockService.Object);
        }
        [Fact]
        public async Task GetJobs_ReturnsOkWithJobs()
        {
            // Arrange
            var jobs = new List<TransportJobReadDto>
                {
                    new() { Id = Guid.NewGuid(), Description = "Job A" },
                    new() { Id = Guid.NewGuid(), Description = "Job B" }
                };

            _mockService.Setup(s => s.GetJobsAsync()).ReturnsAsync(jobs);

            // Act
            var result = await _controller.GetJobs();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedJobs = Assert.IsAssignableFrom<IEnumerable<TransportJobReadDto>>(okResult.Value);
            Assert.Equal(2, returnedJobs.Count());
        }
        [Fact]
        public async Task GetJob_ReturnsNotFound_WhenJobDoesNotExist()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            _mockService.Setup(s => s.GetJobAsync(jobId)).ReturnsAsync((TransportJobReadDto)null);

            // Act
            var result = await _controller.GetJob(jobId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
        [Fact]
        public async Task AcceptJob_ReturnsUnauthorized_WhenUserIdMissing()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var controller = new TransportJobsController(_mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext() // no claims
            };

            // Act
            var result = await controller.AcceptJob(jobId);

            // Assert
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("User identity not found.", unauthorized.Value);
        }
        [Fact]
        public async Task AcceptJob_ReturnsOk_WhenAccepted()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var driverId = "driver123";
            var expectedDto = new TransportJobReadDto { Id = jobId, Description = "Accepted Job" };

            _mockService.Setup(s => s.AcceptJobAsync(jobId, driverId))
                        .ReturnsAsync(OperationResult<TransportJobReadDto>.CreateSuccess(expectedDto));

            var controller = new TransportJobsController(_mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            controller.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, driverId),
                new Claim(ClaimTypes.Role, "Driver")
            }));

            // Act
            var result = await controller.AcceptJob(jobId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDto = Assert.IsType<TransportJobReadDto>(okResult.Value);
            Assert.Equal(jobId, returnedDto.Id);
        }
    }
}
