using CarTransportDashboard.Context;
using System.ComponentModel.DataAnnotations;

namespace CarTransportDashboard.Models.Users
{
    public class AdminProfile:IHasUserId
    {
        [Key]
        public required string UserId { get; set; }


    }
}
