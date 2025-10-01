using CarTransportDashboard.Context;
using CarTransportDashboard.Models;
using CarTransportDashboard.Models.Users;
using CarTransportDashboard.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Threading.Tasks;
using Xunit;
namespace CarTransportDashboard.Tests.Repository;
public class DriverRepositoryTests
{

    [Fact]
    public async Task GetByIdAsync_ReturnsUser_WhenExists()
    {
        // Arrange
        var userId = "user123";
        var user = new ApplicationUser { Id = userId, UserName = "driver1" };

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // ensures isolation
            .Options;

        using var context = new ApplicationDbContext(options);

        context.DriverProfiles.Add(new DriverProfile
        {
            UserId = userId,
            User = user,
            TransportJobs = new List<TransportJob>(),
            LicenseNumber = "LIC123",
        });

        await context.SaveChangesAsync();

        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

        var repo = new DriverRepository(userManagerMock.Object, context);

        // Act
        var result = await repo.GetByIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
    }



    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = "notfound";
        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
        

        userManagerMock.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync((ApplicationUser)null);
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // ensures isolation
        .Options;

        using var context = new ApplicationDbContext(options);

        var repo = new DriverRepository(userManagerMock.Object, context);

        // Act
        var result = await repo.GetByIdAsync(userId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task IsInDriverRoleAsync_ReturnsTrue_WhenUserIsDriver()
    {
        // Arrange
        var userId = "driverId";
        var user = new ApplicationUser { Id = userId, UserName = "driver1" };
        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // ensures isolation
    .Options;

        using var context = new ApplicationDbContext(options);

        userManagerMock.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync(user);
        userManagerMock.Setup(m => m.IsInRoleAsync(user, "Driver")).ReturnsAsync(true);

        var repo = new DriverRepository(userManagerMock.Object, context);

        // Act
        var result = await repo.IsInDriverRoleAsync(userId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsInDriverRoleAsync_ReturnsFalse_WhenUserIsNotDriver()
    {
        // Arrange
        var userId = "driverId";
        var user = new ApplicationUser { Id = userId, UserName = "driver1" };
        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // ensures isolation
    .Options;

        using var context = new ApplicationDbContext(options);

        userManagerMock.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync(user);
        userManagerMock.Setup(m => m.IsInRoleAsync(user, "Driver")).ReturnsAsync(false);

        var repo = new DriverRepository(userManagerMock.Object, context);

        // Act
        var result = await repo.IsInDriverRoleAsync(userId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsInDriverRoleAsync_ReturnsFalse_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = "notfound";
        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // ensures isolation
    .Options;

        using var context = new ApplicationDbContext(options);

        userManagerMock.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync((ApplicationUser)null);

        var repo = new DriverRepository(userManagerMock.Object, context);

        // Act
        var result = await repo.IsInDriverRoleAsync(userId);

        // Assert
        Assert.False(result);
    }
}