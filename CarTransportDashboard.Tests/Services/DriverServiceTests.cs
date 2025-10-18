using CarTransportDashboard.Context;
using CarTransportDashboard.Models;
using CarTransportDashboard.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarTransportDashboard.Tests.Services
{
    public class DriverServiceTests
    {
        private readonly Mock<IDriverRepository> _mockRepo;
        private readonly DriverService _service;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;

        public DriverServiceTests()
        {
            _mockRepo = new Mock<IDriverRepository>();
            _mockUserManager = CreateMockUserManager();
            _service = new DriverService(_mockRepo.Object, _mockUserManager.Object);

        }
        [Fact]
        public async Task GetAssignedJobsAsync_ReturnsMappedJobs_WhenRepositoryReturnsData()
        {
            // Arrange
            var driverId = "driver123";
            var job1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var job2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");

            //mock vehicles assigned to jobs
            var vehicle1 = new Vehicle
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                Make = "Toyota",
                Model = "Camry",
                RegistrationNumber = "ABC123"
            };
            var vehicle2 = new Vehicle
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Make = "Honda",
                Model = "Civic",
                RegistrationNumber = "XYZ789"
            };

            var transportJobs = new List<TransportJob>
                {
                    new TransportJob { Id = job1Id, Description = "Deliver goods", AssignedVehicle=vehicle1 },
                    new TransportJob { Id = job2Id, Description = "Pickup package", AssignedVehicle=vehicle2 }
                };

            _mockRepo.Setup(r => r.GetAssignedJobsAsync(driverId))
                     .ReturnsAsync(transportJobs);

            // Act
            var result = await _service.GetAssignedJobsAsync(driverId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, dto => dto.Description == "Deliver goods");
            Assert.Contains(result, dto => dto.Description == "Pickup package");
        }
        [Fact]
        public async Task GetAssignedJobsAsync_ReturnsEmptyList_WhenNoJobsAssigned()
        {
            // Arrange
            var driverId = "driver456";
            _mockRepo.Setup(r => r.GetAssignedJobsAsync(driverId))
                     .ReturnsAsync(new List<TransportJob>());

            // Act
            var result = await _service.GetAssignedJobsAsync(driverId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
        [Fact]
        public async Task GetAssignedJobsAsync_ThrowsException_WhenRepositoryFails()
        {
            // Arrange
            var driverId = "driver789";
            _mockRepo.Setup(r => r.GetAssignedJobsAsync(driverId))
                     .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.GetAssignedJobsAsync(driverId));
        }

        private static Mock<UserManager<ApplicationUser>> CreateMockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(
                store.Object,
                null, // IOptions<IdentityOptions>
                null, // IPasswordHasher<ApplicationUser>
                new IUserValidator<ApplicationUser>[0],
                new IPasswordValidator<ApplicationUser>[0],
                null, // ILookupNormalizer
                null, // IdentityErrorDescriber
                null, // IServiceProvider
                null  // ILogger<UserManager<ApplicationUser>>
            );
        }


    }
}
