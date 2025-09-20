using CarTransportDashboard.Context;
namespace CarTransportDashboard.Models
{
    public class TransportJob
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public JobStatus Status { get; set; }
        public string PickupLocation { get; set; } = string.Empty;
        public string DropoffLocation { get; set; } = string.Empty;
        public DateTime? ScheduledDate { get; set; }

        // Foreign Keys
        public Guid? AssignedVehicleId { get; set; }
        public Vehicle? AssignedVehicle { get; set; }

        public string? AssignedDriverId { get; set; }
        public ApplicationUser? AssignedDriver { get; set; }
    }

}