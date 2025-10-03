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
            var token = GenerateJwtTokenWithRoles(RoleConstants.Admin); // Simulate wrong role
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/api/driver/jobs");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetAssignedJobs_WithDriverRole_ReturnsOk()
        {
            var token = GenerateJwtTokenWithRoles(RoleConstants.Driver);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/api/driver/jobs");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            // Optionally deserialize and assert payload here
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

    }
}
