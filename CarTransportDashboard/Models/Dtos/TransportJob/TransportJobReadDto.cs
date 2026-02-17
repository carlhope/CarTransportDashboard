using CarTransportDashboard.Context;
using CarTransportDashboard.Models.Dtos.Vehicle;
using CarTransportDashboard.Models.Users;

namespace CarTransportDashboard.Models.Dtos.TransportJob
{
    public record TransportJobReadDto
    {
         public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public JobStatus Status { get; set; }
        public Address PickupLocation { get; set; }
        public Address DropoffLocation { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? AssignedAt { get; set; }
        public DateTime? AcceptedAt { get; set; }
        public float DistanceInMiles { get; set; }
        public TimeSpan EstimatedDuration { get; set; }
        public Polyline? Polyline { get; set; }
        public decimal? CustomerPrice { get; set; }
        public decimal? DriverPayment { get; set; }

        // Foreign Keys
        public Guid? AssignedVehicleId { get; set; }
        public VehicleReadDto? AssignedVehicle { get; set; }

        public string? AssignedDriverId { get; set; }
        public DriverProfile? AssignedDriver { get; set; }
    }
}