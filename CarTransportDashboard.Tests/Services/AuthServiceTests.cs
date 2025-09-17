using CarTransportDashboard.Context;
using CarTransportDashboard.Models;
using CarTransportDashboard.Models.Dtos.Auth;
using CarTransportDashboard.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using Moq;


namespace CarTransportDashboard.Tests.Services;

    public class AuthServiceTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<IConfiguration> _configMock;
        private readonly ApplicationDbContext _dbContext;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userManagerMock = MockUserManager();
            _configMock = new Mock<IConfiguration>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ApplicationDbContext(options);

            _configMock.Setup(c => c["Jwt:Key"]).Returns("9f952c8086b4d8ea6e75e65e562b70724b4c356fb2f771244a0eef291161c37e");
            _configMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
            _configMock.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");

            _authService = new AuthService(_userManagerMock.Object, _configMock.Object, _dbContext);
        }

        private static Mock<UserManager<ApplicationUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);
        }

        [Fact]
        public async Task RegisterAsync_ValidInput_ReturnsUserDto()
        {
            var dto = new RegisterDto { Email = "test@example.com", Password = "Password123!" };

            _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), dto.Password))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _authService.RegisterAsync(dto);

            Assert.NotNull(result);
            Assert.Equal(dto.Email, result.Email);
            Assert.False(string.IsNullOrEmpty(result.AccessToken));
            Assert.False(string.IsNullOrEmpty(result.RefreshToken));
        }

        [Fact]
        public async Task RegisterAsync_Failure_ThrowsException()
        {
            var dto = new RegisterDto { Email = "fail@example.com", Password = "badpass" };

            var errors = new List<IdentityError> { new IdentityError { Description = "Password too weak" } };
            _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), dto.Password))
                .ReturnsAsync(IdentityResult.Failed(errors.ToArray()));

            var ex = await Assert.ThrowsAsync<Exception>(() => _authService.RegisterAsync(dto));
            Assert.Contains("Password too weak", ex.Message);
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsUserDto()
        {
            var user = new ApplicationUser { Id = "user123", Email = "login@example.com", UserName = "login@example.com" };

            _userManagerMock.Setup(m => m.FindByEmailAsync(user.Email)).ReturnsAsync(user);
            _userManagerMock.Setup(m => m.CheckPasswordAsync(user, "Password123!")).ReturnsAsync(true);

            var result = await _authService.LoginAsync(user.Email, "Password123!");

            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);
            Assert.False(string.IsNullOrEmpty(result.AccessToken));
            Assert.False(string.IsNullOrEmpty(result.RefreshToken));
        }

        [Fact]
        public async Task LoginAsync_InvalidCredentials_ReturnsNull()
        {
            _userManagerMock.Setup(m => m.FindByEmailAsync("bad@example.com")).ReturnsAsync((ApplicationUser?)null);

            var result = await _authService.LoginAsync("bad@example.com", "wrongpass");

            Assert.Null(result);
        }

        [Fact]
        public async Task RefreshTokenAsync_ValidToken_ReturnsNewUserDto()
        {
            var user = new ApplicationUser { Id = "user123", Email = "refresh@example.com", UserName = "refresh@example.com" };
            var token = new RefreshToken
            {
                Token = "validtoken",
                UserId = user.Id,
                User = user,
                ExpiryDate = DateTime.UtcNow.AddMinutes(10),
                IsRevoked = false
            };

            _dbContext.RefreshTokens.Add(token);
            await _dbContext.SaveChangesAsync();

            var result = await _authService.RefreshTokenAsync("validtoken");

            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);
            Assert.False(string.IsNullOrEmpty(result.AccessToken));
            Assert.False(string.IsNullOrEmpty(result.RefreshToken));
        }

        [Fact]
        public async Task RefreshTokenAsync_InvalidToken_ReturnsNull()
        {
            var result = await _authService.RefreshTokenAsync("invalidtoken");
            Assert.Null(result);
        }

        [Fact]
        public async Task LogoutAsync_ValidToken_RevokesToken()
        {
            var token = new RefreshToken
            {
                Token = "logouttoken",
                UserId = "user123",
                ExpiryDate = DateTime.UtcNow.AddMinutes(10),
                IsRevoked = false
            };

            _dbContext.RefreshTokens.Add(token);
            await _dbContext.SaveChangesAsync();

            await _authService.LogoutAsync("logouttoken");

            var updated = await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == "logouttoken");
            Assert.True(updated?.IsRevoked);
        }

        [Fact]
        public async Task LogoutAsync_InvalidToken_DoesNothing()
        {
            await _authService.LogoutAsync("nonexistenttoken");

            var token = await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == "nonexistenttoken");
            Assert.Null(token);
        }
    }
