using Microsoft.AspNetCore.Identity;
using CarTransportDashboard.Models;
using CarTransportDashboard.Models.Users;
using CarTransportDashboard.Models.Dtos.Users;

namespace CarTransportDashboard.Context
{

    public class ApplicationUser : IdentityUser
    {
        // Optional: Add custom fields
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PreferredName { get; set; }

        // Navigation
        //public ICollection<TransportJob>? AssignedJobs { get; set; }
        public DriverProfile? DriverProfile { get; set; }
        public AdminProfile? AdminProfile { get; set; }
        public DispatcherProfile? DispatcherProfile { get; set; }

    }
}