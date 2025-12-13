using CarTransportDashboard.Models.Users;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace CarTransportDashboard.Tests.Controller.Driver
{
    public class DriverControllerIntegrationTests:IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public DriverControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAssignedJobs_WithoutDriverRole_ReturnsForbidden()
        {
            var token = GenerateJwtTokenWithRoles(RoleConstants.Admin);
            var request = CreateRequest(HttpMethod.Get, "/api/driver/jobs", token);
            var response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        }

        [Fact]
        public async Task GetAssignedJobs_WithDriverRole_ReturnsOk()
        {
            var token = GenerateJwtTokenWithRoles(RoleConstants.Driver);
            var request = CreateRequest(HttpMethod.Get, "/api/driver/jobs", token);
            var response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetAssignedJobs_MissingReplayHeaders_ReturnsBadRequest()
        {
            var token = GenerateJwtTokenWithRoles(RoleConstants.Driver);
            var request = CreateRequest(HttpMethod.Get, "/api/driver/jobs", token, includeReplayHeaders: false);
            var response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        [Fact]
        public async Task GetAssignedJobs_InvalidTimestampFormat_ReturnsBadRequest()
        {
            var token = GenerateJwtTokenWithRoles(RoleConstants.Driver);
            var request = CreateRequest(HttpMethod.Get, "/api/driver/jobs", token, invalidTimestamp: "not-a-number");
            var response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        [Fact]
        public async Task GetAssignedJobs_TimestampOutsideWindow_ReturnsUnauthorized()
        {
            var token = GenerateJwtTokenWithRoles(RoleConstants.Driver);
            var oldTime = DateTimeOffset.UtcNow.AddMinutes(-5);
            var request = CreateRequest(HttpMethod.Get, "/api/driver/jobs", token, customTimestamp: oldTime);
            var response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        [Fact]
        public async Task GetAssignedJobs_ReplayNonce_ReturnsUnauthorized()
        {
            var token = GenerateJwtTokenWithRoles(RoleConstants.Driver);
            var nonce = Guid.NewGuid().ToString();

            var firstRequest = CreateRequest(HttpMethod.Get, "/api/driver/jobs", token, reuseNonce: nonce);
            var firstResponse = await _client.SendAsync(firstRequest);
            Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);

            var secondRequest = CreateRequest(HttpMethod.Get, "/api/driver/jobs", token, reuseNonce: nonce);
            var secondResponse = await _client.SendAsync(secondRequest);
            Assert.Equal(HttpStatusCode.Unauthorized, secondResponse.StatusCode);
        }

        private string GenerateJwtTokenWithRoles(params string[] roles)
        {
            var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, "driver-123"),
        new(ClaimTypes.Name, "Test Driver")
    };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            var handler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Issuer = "localhost:7286",
                Audience = "localhost:4200",
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes("c9e8278efd9eb2d673ce8bc0a9b2305aecfeb297b56542ebff8f640872f9697b")),
                    SecurityAlgorithms.HmacSha256
                )
            };

            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }
        private HttpRequestMessage CreateRequest(
            HttpMethod method,
            string url,
            string jwtToken,
            bool includeReplayHeaders = true,
            string? reuseNonce = null,
            DateTimeOffset? customTimestamp = null,
            string? invalidTimestamp = null)
                {
                    var request = new HttpRequestMessage(method, url);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                    if (includeReplayHeaders)
                    {
                        // Decide timestamp
                        string timestamp;
                        if (invalidTimestamp != null)
                        {
                            timestamp = invalidTimestamp; // e.g. "not-a-number"
                        }
                        else if (customTimestamp.HasValue)
                        {
                            timestamp = customTimestamp.Value.ToUnixTimeMilliseconds().ToString();
                        }
                        else
                        {
                            timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
                        }

                        // Decide nonce
                        var nonce = reuseNonce ?? Guid.NewGuid().ToString();

                        request.Headers.Add("X-Timestamp", timestamp);
                        request.Headers.Add("X-Nonce", nonce);
                    }

                    return request;
        }

    }
}
