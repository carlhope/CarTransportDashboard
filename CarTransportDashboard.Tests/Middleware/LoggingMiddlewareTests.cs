using CarTransportDashboard.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CarTransportDashboard.Tests.Middleware
{
    public class LoggingMiddlewareTests
    {
        [Fact]
        public async Task LoggingMiddleware_LogsRequestAndResponse()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<LoggingMiddleware>>();
            var middleware = new LoggingMiddleware(
                async (innerHttpContext) => {
                    innerHttpContext.Response.StatusCode = 200;
                    await Task.CompletedTask;
                },
                loggerMock.Object);

            var context = new DefaultHttpContext();
            context.Request.Method = "GET";
            context.Request.Path = "/test";
            context.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "Alice") }, "TestAuth"));

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            loggerMock.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Handling request")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);

            loggerMock.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Finished handling request")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
        [Fact]
        public async Task LoggingMiddleware_DoesNotLogLargeRequest_ForSmallPayload()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<LoggingMiddleware>>();
            var middleware = new LoggingMiddleware(
                async (innerHttpContext) =>
                {
                    innerHttpContext.Response.StatusCode = 200;
                    await Task.CompletedTask;
                },
                loggerMock.Object);

            var context = new DefaultHttpContext();
            context.Request.Method = "POST";
            context.Request.Path = "/upload";
            context.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");
            context.Request.ContentLength = 500; // well below 10 MB
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "Alice") }, "TestAuth"));

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            loggerMock.Verify(
                l => l.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Large request detected")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Never);
        }
        [Fact]
        public async Task LoggingMiddleware_LogsLargeRequest_ForPayloadOverThreshold()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<LoggingMiddleware>>();
            var middleware = new LoggingMiddleware(
                async (innerHttpContext) =>
                {
                    innerHttpContext.Response.StatusCode = 200;
                    await Task.CompletedTask;
                },
                loggerMock.Object);

            var context = new DefaultHttpContext();
            context.Request.Method = "POST";
            context.Request.Path = "/upload";
            context.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");
            context.Request.ContentLength = 11_000_000; // above 10 MB threshold
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "Alice") }, "TestAuth"));

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            loggerMock.Verify(
                l => l.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Large request detected")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
