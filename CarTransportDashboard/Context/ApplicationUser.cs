using Microsoft.AspNetCore.Identity;
using CarTransportDashboard.Models;

namespace CarTransportDashboard.Context
{

    public class ApplicationUser : IdentityUser
    {
        // Optional: Add custom fields
        public string? FullName { get; set; }

        // Navigation
        public ICollection<TransportJob>? AssignedJobs { get; set; }
    }
}