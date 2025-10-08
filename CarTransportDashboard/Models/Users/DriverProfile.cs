using CarTransportDashboard.Context;
using CarTransportDashboard.Models.Dtos.Users;
using System.ComponentModel.DataAnnotations;

namespace CarTransportDashboard.Models.Users
{
    public class DriverProfile: IHasUserId
    {
        [Key]
        public required string UserId { get; set; }

        public required string LicenseNumber { get; set; }
        public DateTime LicenseExpiry { get; set; }
        
    }
}
