using CarTransportDashboard.Context;
using CarTransportDashboard.Models.Dtos.Users;
using System.ComponentModel.DataAnnotations;

namespace CarTransportDashboard.Models.Users
{
    public class DriverProfile: IHasApplicationUser
    {
        [Key]
        public string UserId { get; set; }

        public string LicenseNumber { get; set; }
        public DateTime LicenseExpiry { get; set; }

        public ApplicationUser User { get; set; }
        public ICollection<TransportJob> TransportJobs { get; set; }

    }
}
