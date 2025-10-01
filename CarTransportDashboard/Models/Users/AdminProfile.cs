using CarTransportDashboard.Context;
using System.ComponentModel.DataAnnotations;

namespace CarTransportDashboard.Models.Users
{
    public class AdminProfile
    {
        [Key]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}
