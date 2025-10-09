using CarTransportDashboard.Context;
using CarTransportDashboard.Models.Dtos.Vehicle;

namespace CarTransportDashboard.Models.Dtos.TransportJob
{
    public record TransportJobReadDto
    {
         public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public JobStatus Status { get; set; }
        public string PickupLocation { get; set; } = string.Empty;
        public string DropoffLocation { get; set; } = string.Empty;
        public DateTime? ScheduledDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? AssignedAt { get; set; }
        public DateTime? AcceptedAt { get; set; }

        // Foreign Keys
        public Guid? AssignedVehicleId { get; set; }
        public VehicleReadDto? AssignedVehicle { get; set; }

        public string? AssignedDriverId { get; set; }
        public ApplicationUser? AssignedDriver { get; set; }
    }
}