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

        public string? DispatcherId { get; set; }
        public DispatcherProfile? Dispatcher { get; set; }

        //called from DispatcherProfile.RemoveDriver to keep both sides in sync.
        //Should not be called directly outside that context
        public void UnassignDispatcher()
        {
            DispatcherId = null;
            Dispatcher = null;
        }
    }
}
