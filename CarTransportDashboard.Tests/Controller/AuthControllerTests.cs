using CarTransportDashboard.Controllers;
using CarTransportDashboard.Models.Dtos;
using CarTransportDashboard.Models.Dtos.Auth;
using CarTransportDashboard.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using Xunit;

namespace CarTransportDashboard.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<IWebHostEnvironment> _envMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _envMock = new Mock<IWebHostEnvironment>();
            _envMock.Setup(e => e.EnvironmentName).Returns("Development");

            _controller = new AuthController(_authServiceMock.Object, _envMock.Object);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public async Task Register_ReturnsOkWithUserDto()
        {
            var dto = new RegisterDto { Email = "test@example.com", Password = "Password123!" };
            var expected = new UserDto { Id = "user123", Email = dto.Email };

            _authServiceMock.Setup(s => s.RegisterAsync(dto)).ReturnsAsync(expected);

            var result = await _controller.Register(dto);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var user = Assert.IsType<UserDto>(ok.Value);
            Assert.Equal(dto.Email, user.Email);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOkAndNullsRefreshToken()
        {
            var dto = new LoginDto { Email = "user@example.com", Password = "securepass" };
            var expected = new UserDto { Id = "user123", Email = dto.Email, RefreshToken = "token123" };

            _authServiceMock.Setup(s => s.LoginAsync(dto.Email, dto.Password)).ReturnsAsync(expected);

            var result = await _controller.Login(dto);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var user = Assert.IsType<UserDto>(ok.Value);
            Assert.Equal(dto.Email, user.Email);
            Assert.Null(user.RefreshToken);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            var dto = new LoginDto { Email = "fail@example.com", Password = "wrongpass" };

            _authServiceMock.Setup(s => s.LoginAsync(dto.Email, dto.Password)).ReturnsAsync((UserDto?)null);

            var result = await _controller.Login(dto);

            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]
        public async Task Refresh_ValidToken_ReturnsOkAndNullsRefreshToken()
        {
            var expected = new UserDto { Id = "user123", Email = "refresh@example.com", RefreshToken = "newtoken123" };

            var cookieCollectionMock = new Mock<IRequestCookieCollection>();
            cookieCollectionMock.Setup(c => c["refreshToken"]).Returns("validtoken");

            var cookieFeatureMock = new Mock<IRequestCookiesFeature>();
            cookieFeatureMock.Setup(f => f.Cookies).Returns(cookieCollectionMock.Object);

            var context = new DefaultHttpContext();
            context.Features.Set<IRequestCookiesFeature>(cookieFeatureMock.Object);

            _controller.ControllerContext = new ControllerContext { HttpContext = context };

            _authServiceMock.Setup(s => s.RefreshTokenAsync("validtoken")).ReturnsAsync(expected);

            var result = await _controller.Refresh();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var user = Assert.IsType<UserDto>(ok.Value);
            Assert.Equal(expected.Email, user.Email);
            Assert.Null(user.RefreshToken);
        }

        [Fact]
        public async Task Refresh_MissingToken_ReturnsUnauthorized()
        {
            var result = await _controller.Refresh();
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]
        public async Task Refresh_InvalidToken_ReturnsUnauthorized()
        {
            var cookieCollectionMock = new Mock<IRequestCookieCollection>();
            cookieCollectionMock.Setup(c => c["refreshToken"]).Returns("invalidtoken");

            var cookieFeatureMock = new Mock<IRequestCookiesFeature>();
            cookieFeatureMock.Setup(f => f.Cookies).Returns(cookieCollectionMock.Object);

            var context = new DefaultHttpContext();
            context.Features.Set<IRequestCookiesFeature>(cookieFeatureMock.Object);

            _controller.ControllerContext = new ControllerContext { HttpContext = context };

            _authServiceMock.Setup(s => s.RefreshTokenAsync("invalidtoken")).ReturnsAsync((UserDto?)null);

            var result = await _controller.Refresh();
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]
        public void Logout_ReturnsNoContent()
        {
            var result = _controller.Logout();
            Assert.IsType<NoContentResult>(result);
        }
    }
}
