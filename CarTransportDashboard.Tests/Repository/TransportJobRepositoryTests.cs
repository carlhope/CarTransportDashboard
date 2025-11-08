using CarTransportDashboard.Context;
using CarTransportDashboard.Models;
using CarTransportDashboard.Repository;
using Castle.Core.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        var logger = new Mock<ILogger<TransportJobRepository>>().Object;
        return new TransportJobRepository(context, logger);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsJob_WhenExists()
    {
        var context = GetDbContext();
        var job = TransportJobFactory.CreateBasic();
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
            TransportJobFactory.CreateBasic(title: "A"),
            TransportJobFactory.CreateBasic(title: "B")
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
        var job1 = TransportJobFactory.CreateBasic(title: "A");
        job1.AssignedVehicleId = Guid.NewGuid();

        var job2 = TransportJobFactory.CreateBasic(title: "B");
        job2.AssignedVehicleId = null;

        var job3 = TransportJobFactory.CreateBasic(title: "C");
        job3.AssignedVehicleId = Guid.NewGuid();
        job3.Cancel(); // Set status to Cancelled
        job2.AssignDriver(new ApplicationUser { Id = "driver1", FirstName="john", LastName="Doe" });
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
        var job = TransportJobFactory.CreateBasic(title: "Add");

        await repo.AddAsync(job);

        Assert.Equal(1, context.TransportJobs.Count());
        Assert.Equal(job.Id, context.TransportJobs.First().Id);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesJob()
    {
        var context = GetDbContext();
        var job = TransportJobFactory.CreateBasic(title: "Old");
        context.TransportJobs.Add(job);
        await context.SaveChangesAsync();

        var repo = GetRepository(context);
        job.Title = "New";
        await repo.UpdateAsync(job);

        var updated = context.TransportJobs.Find(job.Id);
        Assert.Equal("New", updated.Title);
    }
}