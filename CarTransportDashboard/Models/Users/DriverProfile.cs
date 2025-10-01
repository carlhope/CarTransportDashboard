using CarTransportDashboard.Context;
using System.ComponentModel.DataAnnotations;

namespace CarTransportDashboard.Models.Users
{
    public class DriverProfile
    {
        [Key]
        public string UserId { get; set; }

        public string LicenseNumber { get; set; }
        public DateTime LicenseExpiry { get; set; }

        public ApplicationUser User { get; set; }
        public ICollection<TransportJob> TransportJobs { get; set; }

    }
}
