using CarTransportDashboard.Context;
using CarTransportDashboard.Models;
using CarTransportDashboard.Repository;
using CarTransportDashboard.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
namespace CarTransportDashboard.Tests.Repository;
public class VehicleRepositoryTests
{
    private ApplicationDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private VehicleRepository GetRepository(ApplicationDbContext context)
    {
        var logger = new Mock<ILogger<VehicleRepository>>().Object;
        return new VehicleRepository(context, logger);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsVehicle_WhenExists()
    {
        var context = GetDbContext();
        var vehicle = new Vehicle { Id = Guid.NewGuid(), Make = "Ford", Model = "Focus", RegistrationNumber = "ABC123" };
        context.Vehicles.Add(vehicle);
        await context.SaveChangesAsync();

        var repo = GetRepository(context);
        var result = await repo.GetByIdAsync(vehicle.Id);

        Assert.NotNull(result);
        Assert.Equal(vehicle.Id, result.Id);
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
    public async Task GetAllAsync_ReturnsAllVehicles()
    {
        var context = GetDbContext();
        context.Vehicles.AddRange(
            new Vehicle { Id = Guid.NewGuid(), Make = "Ford", Model = "Focus", RegistrationNumber = "ABC123" },
            new Vehicle { Id = Guid.NewGuid(), Make = "Toyota", Model = "Corolla", RegistrationNumber = "XYZ789" }
        );
        await context.SaveChangesAsync();

        var repo = GetRepository(context);
        var result = await repo.GetAllAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task AddAsync_AddsVehicle()
    {
        var context = GetDbContext();
        var repo = GetRepository(context);
        var vehicle = new Vehicle { Id = Guid.NewGuid(), Make = "Honda", Model = "Civic", RegistrationNumber = "DEF456" };

        var result = await repo.AddAsync(vehicle);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(vehicle.Id, result.Data.Id);
        Assert.Equal(1, context.Vehicles.Count());
    }

    [Fact]
    public async Task UpdateAsync_UpdatesVehicle()
    {
        var context = GetDbContext();
        var vehicle = new Vehicle { Id = Guid.NewGuid(), Make = "Mazda", Model = "3", RegistrationNumber = "GHI789" };
        context.Vehicles.Add(vehicle);
        await context.SaveChangesAsync();

        var repo = GetRepository(context);
        vehicle.Model = "6";
        var result = await repo.UpdateAsync(vehicle);
        var updated = context.Vehicles.Find(vehicle.Id);

        Assert.True(result.Success);
        Assert.Equal("6", result.Data.Model);
        Assert.Equal("6", updated.Model);
    }

    [Fact]
    public async Task DeleteAsync_RemovesVehicle_WhenExists()
    {
        var context = GetDbContext();
        var vehicle = new Vehicle { Id = Guid.NewGuid(), Make = "BMW", Model = "X5", RegistrationNumber = "JKL012" };
        context.Vehicles.Add(vehicle);
        await context.SaveChangesAsync();

        var repo = GetRepository(context);
        var result = await repo.DeleteAsync(vehicle.Id);

        Assert.True(result.Success);
        Assert.Equal(vehicle.Id, result.Data.Id);
        Assert.Empty(context.Vehicles);
    }

    [Fact]
    public async Task DeleteAsync_DoesNothing_WhenNotExists()
    {
        var context = GetDbContext();
        var repo = GetRepository(context);

        var result = await repo.DeleteAsync(Guid.NewGuid());

        Assert.False(result.Success);
        Assert.Equal("Vehicle not found.", result.Message);
        Assert.Empty(context.Vehicles);
    }

    [Fact]
    public async Task IsAssignedToActiveJobAsync_ReturnsTrue_IfAssignedToActiveJob()
    {
        var context = GetDbContext();
        var vehicleId = Guid.NewGuid();
        context.Vehicles.Add(new Vehicle { Id = vehicleId, Make = "Audi", Model = "A4", RegistrationNumber = "MNO345" });
        context.TransportJobs.Add(new TransportJob
        {
            Id = Guid.NewGuid(),
            Title = "Job1",
            Status = JobStatus.InProgress,
            AssignedVehicleId = vehicleId
        });
        await context.SaveChangesAsync();

        var repo = GetRepository(context);
        var result = await repo.IsAssignedToActiveJobAsync(vehicleId);

        Assert.True(result);
    }

    [Fact]
    public async Task IsAssignedToActiveJobAsync_ReturnsFalse_IfNotAssignedOrJobCompleted()
    {
        var context = GetDbContext();
        var vehicleId = Guid.NewGuid();
        context.Vehicles.Add(new Vehicle { Id = vehicleId, Make = "Audi", Model = "A4", RegistrationNumber = "MNO345" });
        context.TransportJobs.Add(new TransportJob
        {
            Id = Guid.NewGuid(),
            Title = "Job1",
            Status = JobStatus.Completed,
            AssignedVehicleId = vehicleId
        });
        await context.SaveChangesAsync();

        var repo = GetRepository(context);
        var result = await repo.IsAssignedToActiveJobAsync(vehicleId);

        Assert.False(result);
    }
}