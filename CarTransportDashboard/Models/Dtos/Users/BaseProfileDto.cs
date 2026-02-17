using CarTransportDashboard.Context;
using Shared.Models;

namespace CarTransportDashboard.Models.Dtos.Users
{
    public class BaseProfileDto
    {
        public required string UserId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string DisplayName { get; set; }
        public required string Email { get; set; }
    }

}
