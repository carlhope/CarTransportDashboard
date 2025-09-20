using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarTransportDashboard.Context;
using CarTransportDashboard.Models;
using CarTransportDashboard.Repository;
using Microsoft.EntityFrameworkCore;
using Xunit;
namespace CarTransportDashboard.Tests.Repository;
public class TransportJobRepositoryTests
{
    private ApplicationDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private TransportJobRepository GetRepository(ApplicationDbContext context)
    {
        return new TransportJobRepository(context);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsJob_WhenExists()
    {
        var context = GetDbContext();
        var job = new TransportJob { Id = Guid.NewGuid(), Title = "Test" };
        context.TransportJobs.Add(job);
        await context.SaveChangesAsync();

        var repo = GetRepository(context);
        var result = await repo.GetByIdAsync(job.Id);

        Assert.NotNull(result);
        Assert.Equal(job.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        var context = GetDbContext();
        var repo = GetRepository(context);

        var result = await repo.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllJobs()
    {
        var context = GetDbContext();
        context.TransportJobs.AddRange(
            new TransportJob { Id = Guid.NewGuid(), Title = "A" },
            new TransportJob { Id = Guid.NewGuid(), Title = "B" }
        );
        await context.SaveChangesAsync();

        var repo = GetRepository(context);
        var result = await repo.GetAllAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAvailableJobsAsync_ReturnsJobsWithVehicleNoDriver()
    {
        var context = GetDbContext();
        var job1 = new TransportJob { Id = Guid.NewGuid(), Title = "A", AssignedVehicleId = Guid.NewGuid(), AssignedDriverId = null };
        var job2 = new TransportJob { Id = Guid.NewGuid(), Title = "B", AssignedVehicleId = null, AssignedDriverId = null };
        var job3 = new TransportJob { Id = Guid.NewGuid(), Title = "C", AssignedVehicleId = Guid.NewGuid(), AssignedDriverId = Guid.NewGuid().ToString() };
        context.TransportJobs.AddRange(job1, job2, job3);
        await context.SaveChangesAsync();

        var repo = GetRepository(context);
        var result = await repo.GetAvailableJobsAsync();

        Assert.Single(result);
        Assert.Equal(job1.Id, result.First().Id);
    }

    [Fact]
    public async Task AddAsync_AddsJob()
    {
        var context = GetDbContext();
        var repo = GetRepository(context);
        var job = new TransportJob { Id = Guid.NewGuid(), Title = "Add" };

        await repo.AddAsync(job);

        Assert.Equal(1, context.TransportJobs.Count());
        Assert.Equal(job.Id, context.TransportJobs.First().Id);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesJob()
    {
        var context = GetDbContext();
        var job = new TransportJob { Id = Guid.NewGuid(), Title = "Old" };
        context.TransportJobs.Add(job);
        await context.SaveChangesAsync();

        var repo = GetRepository(context);
        job.Title = "New";
        await repo.UpdateAsync(job);

        var updated = context.TransportJobs.Find(job.Id);
        Assert.Equal("New", updated.Title);
    }

    [Fact]
    public async Task AssignVehicleAsync_AssignsVehicle_WhenJobExists()
    {
        var context = GetDbContext();
        var job = new TransportJob { Id = Guid.NewGuid(), Title = "Job" };
        context.TransportJobs.Add(job);
        await context.SaveChangesAsync();

        var repo = GetRepository(context);
        var vehicleId = Guid.NewGuid();
        await repo.AssignVehicleAsync(job.Id, vehicleId);

        var updated = context.TransportJobs.Find(job.Id);
        Assert.Equal(vehicleId, updated.AssignedVehicleId);
    }

    [Fact]
    public async Task AssignVehicleAsync_DoesNothing_WhenJobNotExists()
    {
        var context = GetDbContext();
        var repo = GetRepository(context);

        await repo.AssignVehicleAsync(Guid.NewGuid(), Guid.NewGuid());

        Assert.Empty(context.TransportJobs);
    }

    [Fact]
    public async Task AssignDriverAsync_AssignsDriver_WhenJobExists()
    {
        var context = GetDbContext();
        var job = new TransportJob { Id = Guid.NewGuid(), Title = "Job" };
        context.TransportJobs.Add(job);
        await context.SaveChangesAsync();

        var repo = GetRepository(context);
        var driverId = Guid.NewGuid().ToString();
        await repo.AssignDriverAsync(job.Id, driverId);

        var updated = context.TransportJobs.Find(job.Id);
        Assert.Equal(driverId, updated.AssignedDriverId);
    }

    [Fact]
    public async Task AssignDriverAsync_DoesNothing_WhenJobNotExists()
    {
        var context = GetDbContext();
        var repo = GetRepository(context);

        await repo.AssignDriverAsync(Guid.NewGuid(), Guid.NewGuid().ToString());

        Assert.Empty(context.TransportJobs);
    }
}