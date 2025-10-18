using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarTransportDashboard.Context;
using CarTransportDashboard.Models;
using CarTransportDashboard.Models.Dtos.TransportJob;
using CarTransportDashboard.Models.Dtos.Vehicle;
using CarTransportDashboard.Repository.Interfaces;
using CarTransportDashboard.Services;
using Moq;
using Xunit;
namespace CarTransportDashboard.Tests.Services;
public class TransportJobServiceTests
{
    private readonly Mock<ITransportJobRepository> _jobRepoMock = new();
    private readonly Mock<IVehicleRepository> _vehicleRepoMock = new();
    private readonly Mock<IDriverRepository> _driverRepoMock = new();
    private readonly Mock<IDriverService> _driverServiceMock = new();
    private Vehicle TestVehicle => new Vehicle
    {
        Id = Guid.NewGuid(),
        Make = "TestMake",
        Model = "TestModel",
        RegistrationNumber = "TEST123"
    };

    private TransportJobService CreateService() =>
        new TransportJobService(_jobRepoMock.Object, _vehicleRepoMock.Object, _driverRepoMock.Object, _driverServiceMock.Object);

    [Fact]
    public async Task GetJobAsync_ReturnsDto_WhenJobExists()
    {
        var jobId = Guid.NewGuid();
        var job = new TransportJob { Id = jobId, Title = "Test", AssignedVehicle = TestVehicle };
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
        TransportJob inProgressJob = new TransportJob { Id = Guid.NewGuid(), Title = "B", AssignedVehicle=TestVehicle};
        inProgressJob.AssignDriver(new ApplicationUser { Id = Guid.NewGuid().ToString(), FirstName="Jane", LastName="Smith"});
        inProgressJob.AcceptJob(); //sets status to in progress
        var jobs = new List<TransportJob>
        {
            new TransportJob { Id = Guid.NewGuid(), Title = "A", AssignedVehicle = TestVehicle   },//status defaults to Available
            inProgressJob
        };
        _jobRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(jobs);

        var service = CreateService();
        var result = await service.GetJobsAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAvailableJobsAsync_ReturnsDtos()
    {
        var jobs = new List<TransportJob>
        {
            new TransportJob { Id = Guid.NewGuid(), Title = "A", AssignedVehicle = TestVehicle }//status defaults to Available
        };
        _jobRepoMock.Setup(r => r.GetAvailableJobsAsync()).ReturnsAsync(jobs);

        var service = CreateService();
        var result = await service.GetAvailableJobsAsync();

        Assert.Single(result);
    }

    [Fact]
    public async Task AcceptJobAsync_UpdatesJobStatusAndDriver()
    {
        var jobId = Guid.NewGuid();
        var service = CreateService();
        var driver = Guid.NewGuid().ToString();
        var job = new TransportJob { Id = jobId, AssignedVehicle = TestVehicle };
        job.AssignDriver(new ApplicationUser() { Id=driver, FirstName="John", LastName="Doe"});
        _jobRepoMock.Setup(r => r.GetByIdAsync(jobId)).ReturnsAsync(job);

      
        await service.AcceptJobAsync(jobId, driver);

        Assert.Equal(driver, job.AssignedDriverId);
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

    //[Fact]
    //public async Task UpdateJobStatusAsync_UpdatesStatus()
    //{
    //    var jobId = Guid.NewGuid();
    //    var job = new TransportJob { Id = jobId, Status = JobStatus.Available };
    //    _jobRepoMock.Setup(r => r.GetByIdAsync(jobId)).ReturnsAsync(job);

    //    var service = CreateService();
    //    await service.UpdateJobStatusAsync(jobId, JobStatus.Completed);

    //    Assert.Equal(JobStatus.Completed, job.Status);
    //    _jobRepoMock.Verify(r => r.UpdateAsync(job), Times.Once);
    //}

    [Fact]
    public async Task AssignVehicleToJobAsync_UpdatesVehicle_WhenBothExist()
    {
        var jobId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();
        var vehicle = new Vehicle { Id = vehicleId };
        var job = new TransportJob { Id = jobId, AssignedVehicle=vehicle };
        
        _jobRepoMock.Setup(r => r.GetByIdAsync(jobId)).ReturnsAsync(job);
        _vehicleRepoMock.Setup(r => r.GetByIdAsync(vehicleId)).ReturnsAsync(vehicle);

        var service = CreateService();
        await service.AssignVehicleToJobAsync(jobId, vehicleId);

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
        var jobId = Guid.NewGuid();
        var driverId = Guid.NewGuid().ToString();
        var job = new TransportJob { Id = jobId, AssignedVehicle = TestVehicle }; //status defaults to Available
        _jobRepoMock.Setup(r => r.GetByIdAsync(jobId)).ReturnsAsync(job);
        _jobRepoMock.Setup(r => r.UpdateAsync(job)).ReturnsAsync(OperationResult<TransportJob>.CreateSuccess(job));
        _driverRepoMock.Setup(r => r.IsInDriverRoleAsync(driverId.ToString())).ReturnsAsync(true);
        _driverRepoMock.Setup(r => r.GetAssignedJobsAsync(driverId)).ReturnsAsync(Enumerable.Empty<TransportJob>());
        _driverServiceMock.Setup(s => s.GetDriverUserByIdAsync(driverId)).ReturnsAsync(new ApplicationUser { Id = driverId, FirstName="John", LastName="Doe" });

        var service = CreateService();
        await service.AssignDriverToJobAsync(jobId, driverId);

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
        var dto = new TransportJobCreateDto { Id = Guid.NewGuid(), Title = "New Job", AssignedVehicle= vehicleDto, AssignedDriverId=vehicleDto.Id.ToString()};
        TransportJob? addedJob = null;
        _jobRepoMock.Setup(r => r.AddAsync(It.IsAny<TransportJob>()))
             .ReturnsAsync((TransportJob j) =>
             {
                 addedJob = j;
                 return OperationResult<TransportJob>.CreateSuccess(j);
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
        var jobId = Guid.NewGuid();
        var job = new TransportJob { Id = jobId, Title = "Old", AssignedVehicle = TestVehicle };//status defaults to Available
       var testVehicleDto = new VehicleWriteDto
        {
            Id = Guid.NewGuid(),
            Make = "Toyota",
            Model = "Corolla",
            RegistrationNumber = "XYZ789"
        };
        var dto = new TransportJobUpdateDto { Id = jobId, Title = "Updated", AssignedVehicle = testVehicleDto};

        _jobRepoMock.Setup(r => r.GetByIdAsync(jobId)).ReturnsAsync(job);
        _jobRepoMock.Setup(r => r.UpdateAsync(It.IsAny<TransportJob>()))
            .ReturnsAsync((TransportJob updatedJob) => OperationResult<TransportJob>.CreateSuccess(updatedJob));

        var service = CreateService();
        var result = await service.UpdateJobAsync(jobId, dto);

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