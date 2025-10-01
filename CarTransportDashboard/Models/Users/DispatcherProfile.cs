using CarTransportDashboard.Context;
using System.ComponentModel.DataAnnotations;

namespace CarTransportDashboard.Models.Users
{
    public class DispatcherProfile
    {
        [Key]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

    }
}
