using CarTransportDashboard.Context;
using CarTransportDashboard.Models.Dtos.TransportJob;

namespace CarTransportDashboard.Models.Dtos.Users
{
    public class DriverDto: BaseProfileDto
    {

        public string? LicenseNumber { get; set; }
        public DateTime LicenseExpiry { get; set; }
    }
}
