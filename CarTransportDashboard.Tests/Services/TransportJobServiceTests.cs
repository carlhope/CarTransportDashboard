using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarTransportDashboard.Models;
using CarTransportDashboard.Models.Dtos.TransportJob;
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

    private TransportJobService CreateService() =>
        new TransportJobService(_jobRepoMock.Object, _vehicleRepoMock.Object, _driverRepoMock.Object);

    [Fact]
    public async Task GetJobAsync_ReturnsDto_WhenJobExists()
    {
        var jobId = Guid.NewGuid();
        var job = new TransportJob { Id = jobId, Title = "Test", Status = JobStatus.Available };
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
        var jobs = new List<TransportJob>
        {
            new TransportJob { Id = Guid.NewGuid(), Title = "A", Status = JobStatus.Available },
            new TransportJob { Id = Guid.NewGuid(), Title = "B", Status = JobStatus.InProgress }
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
            new TransportJob { Id = Guid.NewGuid(), Title = "A", Status = JobStatus.Available }
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
        var job = new TransportJob { Id = jobId, Status = JobStatus.Available };
        _jobRepoMock.Setup(r => r.GetByIdAsync(jobId)).ReturnsAsync(job);

        var service = CreateService();
        var driver = Guid.NewGuid().ToString();
        await service.AcceptJobAsync(jobId, driver);

        Assert.Equal(driver, job.AssignedDriverId);
        Assert.Equal(JobStatus.InProgress, job.Status);
        _jobRepoMock.Verify(r => r.UpdateAsync(job), Times.Once);
    }

    [Fact]
    public async Task AcceptJobAsync_Throws_WhenJobNotFound()
    {
        var jobId = Guid.NewGuid();
        _jobRepoMock.Setup(r => r.GetByIdAsync(jobId)).ReturnsAsync((TransportJob)null);

        var service = CreateService();
        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.AcceptJobAsync(jobId, Guid.NewGuid().ToString()));
    }

    [Fact]
    public async Task UpdateJobStatusAsync_UpdatesStatus()
    {
        var jobId = Guid.NewGuid();
        var job = new TransportJob { Id = jobId, Status = JobStatus.Available };
        _jobRepoMock.Setup(r => r.GetByIdAsync(jobId)).ReturnsAsync(job);

        var service = CreateService();
        await service.UpdateJobStatusAsync(jobId, JobStatus.Completed);

        Assert.Equal(JobStatus.Completed, job.Status);
        _jobRepoMock.Verify(r => r.UpdateAsync(job), Times.Once);
    }

    [Fact]
    public async Task AssignVehicleToJobAsync_UpdatesVehicle_WhenBothExist()
    {
        var jobId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();
        var job = new TransportJob { Id = jobId };
        var vehicle = new Vehicle { Id = vehicleId };
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
        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.AssignVehicleToJobAsync(jobId, vehicleId));
    }

    [Fact]
    public async Task AssignDriverToJobAsync_UpdatesDriver_WhenJobExistsAndIsDriver()
    {
        var jobId = Guid.NewGuid();
        var driverId = Guid.NewGuid().ToString();
        var job = new TransportJob { Id = jobId };
        _jobRepoMock.Setup(r => r.GetByIdAsync(jobId)).ReturnsAsync(job);
        _driverRepoMock.Setup(r => r.IsInDriverRoleAsync(driverId.ToString())).ReturnsAsync(true);

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
        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.AssignDriverToJobAsync(jobId, driverId));
    }

    [Fact]
    public async Task CreateJobAsync_AddsJobAndReturnsDto()
    {
        var dto = new TransportJobWriteDto { Id = Guid.NewGuid(), Title = "New Job", Status = JobStatus.Available };
        TransportJob? addedJob = null;
        _jobRepoMock.Setup(r => r.AddAsync(It.IsAny<TransportJob>()))
            .Callback<TransportJob>(j => addedJob = j)
            .Returns(Task.CompletedTask);

        var service = CreateService();
        var result = await service.CreateJobAsync(dto);

        Assert.NotNull(result);
        Assert.Equal(dto.Title, result.Title);
        Assert.Equal(dto.Status, result.Status);
        Assert.Equal(dto.Id, result.Id);
        Assert.NotNull(addedJob);
    }

    [Fact]
    public async Task UpdateJobAsync_UpdatesJob_WhenExists()
    {
        var jobId = Guid.NewGuid();
        var job = new TransportJob { Id = jobId, Title = "Old", Status = JobStatus.Available };
        var dto = new TransportJobWriteDto { Id = jobId, Title = "Updated", Status = JobStatus.InProgress };
        _jobRepoMock.Setup(r => r.GetByIdAsync(jobId)).ReturnsAsync(job);

        var service = CreateService();
        await service.UpdateJobAsync(jobId, dto);

        Assert.Equal("Updated", job.Title);
        Assert.Equal(JobStatus.InProgress, job.Status);
        _jobRepoMock.Verify(r => r.UpdateAsync(job), Times.Once);
    }

    [Fact]
    public async Task UpdateJobAsync_Throws_WhenJobNotFound()
    {
        var jobId = Guid.NewGuid();
        var dto = new TransportJobWriteDto { Id = jobId, Title = "Updated", Status = JobStatus.InProgress };
        _jobRepoMock.Setup(r => r.GetByIdAsync(jobId)).ReturnsAsync((TransportJob)null);

        var service = CreateService();
        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateJobAsync(jobId, dto));
    }
}