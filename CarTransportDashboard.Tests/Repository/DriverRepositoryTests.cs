using System.Threading.Tasks;
using CarTransportDashboard.Context;
using CarTransportDashboard.Models;
using CarTransportDashboard.Repository;
using Microsoft.AspNetCore.Identity;
using Moq;
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
        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

        userManagerMock.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync(user);

        var repo = new DriverRepository(userManagerMock.Object);

        // Act
        var result = await repo.GetByIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = "notfound";
        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

        userManagerMock.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync((ApplicationUser)null);

        var repo = new DriverRepository(userManagerMock.Object);

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

        userManagerMock.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync(user);
        userManagerMock.Setup(m => m.IsInRoleAsync(user, "Driver")).ReturnsAsync(true);

        var repo = new DriverRepository(userManagerMock.Object);

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

        userManagerMock.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync(user);
        userManagerMock.Setup(m => m.IsInRoleAsync(user, "Driver")).ReturnsAsync(false);

        var repo = new DriverRepository(userManagerMock.Object);

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

        userManagerMock.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync((ApplicationUser)null);

        var repo = new DriverRepository(userManagerMock.Object);

        // Act
        var result = await repo.IsInDriverRoleAsync(userId);

        // Assert
        Assert.False(result);
    }
}