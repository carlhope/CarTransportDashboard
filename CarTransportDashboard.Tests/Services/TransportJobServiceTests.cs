using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarTransportDashboard.Context;
using CarTransportDashboard.Helpers;
using CarTransportDashboard.Models;
using CarTransportDashboard.Models.Dtos.TransportJob;
using CarTransportDashboard.Models.Dtos.Vehicle;
using CarTransportDashboard.Repository.Interfaces;
using CarTransportDashboard.Services;
using CarTransportDashboard.Services.Interfaces;
using Castle.Core.Logging;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using CarTransportDashboard.Models.Dtos.Routes;
namespace CarTransportDashboard.Tests.Services;
public class TransportJobServiceTests
{
    private readonly Mock<ITransportJobRepository> _jobRepoMock = new();
    private readonly Mock<IVehicleRepository> _vehicleRepoMock = new();
    private readonly Mock<IDriverRepository> _driverRepoMock = new();
    private readonly Mock<IDriverService> _driverServiceMock = new();
    private readonly Mock<ILogger<TransportJobService>> _loggerMock = new();
    private readonly Mock<IRouteService> _routeServiceMock = new();
    private readonly Mock<IEmailService> _emailServiceMock = new();
    private Vehicle TestVehicle => new Vehicle
    {
        Id = Guid.NewGuid(),
        Make = "TestMake",
        Model = "TestModel",
        RegistrationNumber = "TEST123"
    };
    private Address MockPickupAddress = new Address
    {
        CompanyName = "Acme Supplies Ltd",
        AddressLine1 = "Unit 4, Acme Business Park",
        AddressLine2 = "Warehouse Entrance",
        Locality = "Stoke-on-Trent",
        PostalCode = "ST1 1AA",
        Country = "GB",
        Lat = 53.0027,
        Lng = -2.1794
    };

    private Address MockDropoffAddress = new Address
    {
        CompanyName = "Derby Distribution Hub",
        AddressLine1 = "456 Industrial Estate",
        AddressLine2 = "Loading Bay 3",
        Locality = "Derby",
        PostalCode = "DE1 2BB",
        Country = "GB",
        Lat = 52.9225,
        Lng = -1.4746
    };

    private TransportJobService CreateService() =>
        new TransportJobService(_jobRepoMock.Object, _vehicleRepoMock.Object, _driverRepoMock.Object, _driverServiceMock.Object, _loggerMock.Object, _routeServiceMock.Object, _emailServiceMock.Object);

    [Fact]
    public async Task GetJobAsync_ReturnsDto_WhenJobExists()
    {
        var jobId = Guid.NewGuid();
        var job = TransportJobFactory.CreateBasic(title: "Test");
        job.Id = jobId;
        job.AssignedVehicle = TestVehicle;
        job.AssignedVehicleId = TestVehicle.Id;
        _jobRepoMock.Setup(r => r.GetByIdAsync(jobId)).ReturnsAsync(job);

        var service = CreateService();
        var result = await service.GetJobAsync(jobId);

        Assert.NotNull(result);
        Assert.Equal(jobId, result.Id);
    }

    [Fact]
    public async Task GetJobAsync_ReturnsNull_WhenJobNotFound()
    {
        var jobId = Guid.NewGuid();
        _jobRepoMock.Setup(r => r.GetByIdAsync(jobId)).ReturnsAsync((TransportJob)null);

        var service = CreateService();
        var result = await service.GetJobAsync(jobId);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetJobsAsync_ReturnsDtos()
    {
        // Arrange
        var availableJob = TransportJobFactory.CreateBasic(title: "A");
        availableJob.AssignedVehicle = TestVehicle;
        availableJob.AssignedVehicleId = TestVehicle.Id;

        var inProgressJob = TransportJobFactory.CreateBasic(title: "B");
        inProgressJob.AssignedVehicle = TestVehicle;
        inProgressJob.AssignedVehicleId = TestVehicle.Id;
        inProgressJob.AssignDriver(new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            FirstName = "Jane",
            LastName = "Smith"
        });
        inProgressJob.AcceptJob(); // sets status to InProgress

        var jobs = new List<TransportJob> { availableJob, inProgressJob };
        _jobRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(jobs);

        var service = CreateService();

        // Act
        var result = await service.GetJobsAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAvailableJobsAsync_ReturnsDtos()
    {
        // Arrange
        var availableJob = TransportJobFactory.CreateBasic(title: "A");
        availableJob.AssignedVehicle = TestVehicle;
        availableJob.AssignedVehicleId = TestVehicle.Id;

        var jobs = new List<TransportJob> { availableJob };
        _jobRepoMock.Setup(r => r.GetAvailableJobsAsync()).ReturnsAsync(jobs);

        var service = CreateService();

        // Act
        var result = await service.GetAvailableJobsAsync();

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task AcceptJobAsync_UpdatesJobStatusAndDriver()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var service = CreateService();
        var driverId = Guid.NewGuid().ToString();

        var job = TransportJobFactory.CreateBasic();
        job.Id = jobId;
        job.AssignedVehicle = TestVehicle;
        job.AssignedVehicleId = TestVehicle.Id;
        job.AssignDriver(new ApplicationUser
        {
            Id = driverId,
            FirstName = "John",
            LastName = "Doe"
        });

        _jobRepoMock.Setup(r => r.GetByIdAsync(jobId)).ReturnsAsync(job);
        _routeServiceMock.Setup(r => r.GetRouteInfoAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new RouteEstimateDto
            {
                DistanceInMiles = 100,
                EstimatedDuration = TimeSpan.FromHours(2),
                RoutePreviewUrl = "http://testroute.com"
            });

        // Act
        await service.AcceptJobAsync(jobId, driverId);

        // Assert
        Assert.Equal(driverId, job.AssignedDriverId);
        Assert.Equal(JobStatus.InProgress, job.Status);
        _jobRepoMock.Verify(r => r.UpdateAsync(job), Times.Once);
    }

    [Fact]
    public async Task AcceptJobAsync_Throws_WhenJobNotFound()
    {
        var jobId = Guid.NewGuid();
        var driverId = Guid.NewGuid().ToString();
        _jobRepoMock.Setup(r => r.GetByIdAsync(jobId)).ReturnsAsync((TransportJob)null);

        var service = CreateService();
        var result = await service.AcceptJobAsync(jobId, driverId);
        Assert.False(result.Success);
        Assert.Equal("Transport job not found.", result.Message);
    }

    [Fact]
    public async Task AssignVehicleToJobAsync_UpdatesVehicle_WhenBothExist()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();

        var vehicle = new Vehicle { Id = vehicleId };

        var job = TransportJobFactory.CreateBasic();
        job.Id = jobId;
        job.AssignedVehicle = vehicle;
        job.AssignedVehicleId = vehicle.Id;

        _jobRepoMock.Setup(r => r.GetByIdAsync(jobId)).ReturnsAsync(job);
        _vehicleRepoMock.Setup(r => r.GetByIdAsync(vehicleId)).ReturnsAsync(vehicle);

        var service = CreateService();

        // Act
        await service.AssignVehicleToJobAsync(jobId, vehicleId);

        // Assert
        Assert.Equal(vehicleId, job.AssignedVehicleId);
        _jobRepoMock.Verify(r => r.UpdateAsync(job), Times.Once);
    }

    [Fact]
    public async Task AssignVehicleToJobAsync_Throws_WhenJobOrVehicleNotFound()
    {
        var jobId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();
        _jobRepoMock.Setup(r => r.GetByIdAsync(jobId)).ReturnsAsync((TransportJob)null);
        _vehicleRepoMock.Setup(r => r.GetByIdAsync(vehicleId)).ReturnsAsync((Vehicle)null);

        var service = CreateService();
        var result = await service.AssignVehicleToJobAsync(jobId, vehicleId);
        Assert.False(result.Success);
        Assert.Equal("Job or vehicle not found.", result.Message);
    }

    [Fact]
    public async Task AssignDriverToJobAsync_UpdatesDriver_WhenJobExistsAndIsDriver()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var driverId = Guid.NewGuid().ToString();

        var job = TransportJobFactory.CreateBasic();
        job.Id = jobId;
        job.AssignedVehicle = TestVehicle;
        job.AssignedVehicleId = TestVehicle.Id;

        _jobRepoMock.Setup(r => r.GetByIdAsync(jobId)).ReturnsAsync(job);
        _jobRepoMock.Setup(r => r.UpdateAsync(job)).ReturnsAsync(OperationResult<TransportJob>.CreateSuccess(job));
        _driverRepoMock.Setup(r => r.IsInDriverRoleAsync(driverId)).ReturnsAsync(true);
        _driverRepoMock.Setup(r => r.GetAssignedJobsAsync(driverId)).ReturnsAsync(Enumerable.Empty<TransportJob>());
        _driverServiceMock.Setup(s => s.GetDriverUserByIdAsync(driverId)).ReturnsAsync(new ApplicationUser
        {
            Id = driverId,
            FirstName = "John",
            LastName = "Doe"
        });

        _routeServiceMock.Setup(r => r.GetRouteInfoAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new RouteEstimateDto
            {
                DistanceInMiles = 100,
                EstimatedDuration = TimeSpan.FromHours(2),
                RoutePreviewUrl = "http://testroute.com"
            });

        var service = CreateService();

        // Act
        await service.AssignDriverToJobAsync(jobId, driverId);

        // Assert
        Assert.Equal(driverId, job.AssignedDriverId);
        _jobRepoMock.Verify(r => r.UpdateAsync(job), Times.Once);
    }

    [Fact]
    public async Task AssignDriverToJobAsync_Throws_WhenJobNotFoundOrNotDriver()
    {
        var jobId = Guid.NewGuid();
        var driverId = Guid.NewGuid().ToString();
        _jobRepoMock.Setup(r => r.GetByIdAsync(jobId)).ReturnsAsync((TransportJob)null);
        _driverRepoMock.Setup(r => r.IsInDriverRoleAsync(driverId.ToString())).ReturnsAsync(false);

        var service = CreateService();
        var result = await service.AssignDriverToJobAsync(jobId, driverId);
        Assert.False(result.Success);
        Assert.Equal("Job not found or user is not a driver.", result.Message);
    }

    [Fact]
    public async Task CreateJobAsync_AddsJobAndReturnsDto()
    {
        var vehicleDto = new VehicleWriteDto
        {
            Id = Guid.NewGuid(),
            Make = "Ford",
            Model = "Focus",
            RegistrationNumber = "ABC123"
        };
        var dto = new TransportJobCreateDto { 
            Id = Guid.NewGuid(), 
            Title = "New Job",
            DropoffLocation=MockPickupAddress,
            PickupLocation=MockDropoffAddress,
            Description="Test Description",
            AssignedVehicle = vehicleDto, 
            AssignedDriverId=vehicleDto.Id.ToString(),
        };
        TransportJob? addedJob = null;
        _jobRepoMock.Setup(r => r.AddAsync(It.IsAny<TransportJob>()))
             .ReturnsAsync((TransportJob j) =>
             {
                 addedJob = j;
                 return OperationResult<TransportJob>.CreateSuccess(j);
             });
        _routeServiceMock.Setup(r => r.GetRouteInfoAsync(It.IsAny<string>(), It.IsAny<string>()))
           .ReturnsAsync(new RouteEstimateDto
           {
               DistanceInMiles = 100,
               EstimatedDuration = TimeSpan.FromHours(2),
               RoutePreviewUrl = "http://testroute.com"
           });

        var service = CreateService();
        var result = await service.CreateJobAsync(dto);

        Assert.NotNull(result);
        Assert.Equal(dto.Title, result.Data.Title);
        Assert.NotNull(addedJob);
    }

    [Fact]
    public async Task UpdateJobAsync_UpdatesJob_WhenExists()
    {
        // Arrange
        var jobId = Guid.NewGuid();

        var job = TransportJobFactory.CreateBasic(
            title: "Old",
            description: "Old Description",
            pickup: MockPickupAddress,
            dropoff: MockDropoffAddress
        );
        job.Id = jobId;
        job.AssignedVehicle = TestVehicle;
        job.AssignedVehicleId = TestVehicle.Id;
        job.DistanceInMiles = 50.0F;
        job.DriverPayment = 30;
        job.CustomerPrice = 50;

        var testVehicleDto = new VehicleWriteDto
        {
            Id = Guid.NewGuid(),
            Make = "Toyota",
            Model = "Corolla",
            RegistrationNumber = "XYZ789"
        };

        var dto = new TransportJobUpdateDto
        {
            Id = jobId,
            Title = "Updated",
            AssignedVehicle = testVehicleDto,
            Description = "Updated Description",
            PickupLocation = MockDropoffAddress,//swapped to minic a change
            DropoffLocation = MockPickupAddress //swapped to minic a change
        };

        _jobRepoMock.Setup(r => r.GetByIdAsync(jobId)).ReturnsAsync(job);
        _jobRepoMock.Setup(r => r.UpdateAsync(It.IsAny<TransportJob>()))
            .ReturnsAsync((TransportJob updatedJob) => OperationResult<TransportJob>.CreateSuccess(updatedJob));

        _routeServiceMock.Setup(r => r.GetRouteInfoAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new RouteEstimateDto
            {
                DistanceInMiles = 100,
                EstimatedDuration = TimeSpan.FromHours(2),
                RoutePreviewUrl = "http://testroute.com"
            });

        var service = CreateService();

        // Act
        var result = await service.UpdateJobAsync(jobId, dto);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("Updated", result.Data.Title);
        _jobRepoMock.Verify(r => r.UpdateAsync(job), Times.Once);
    }

    [Fact]
    public async Task UpdateJobAsync_Throws_WhenJobNotFound()
    {
        var jobId = Guid.NewGuid();
        var dto = new TransportJobUpdateDto { Id = jobId, Title = "Updated"};
        _jobRepoMock.Setup(r => r.GetByIdAsync(jobId)).ReturnsAsync((TransportJob)null);

        var service = CreateService();
        var result = await service.UpdateJobAsync(jobId, dto);
        Assert.False(result.Success);
        Assert.Equal("Transport job not found.", result.Message);
    }
}