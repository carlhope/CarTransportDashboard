using CarTransportDashboard.Context;
using CarTransportDashboard.Models.Dtos.Vehicle;

namespace CarTransportDashboard.Models.Dtos.TransportJob
{
    public record TransportJobWriteDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public JobStatus Status { get; set; }

        // Foreign Keys
        public Guid? AssignedVehicleId { get; set; }
        public VehicleWriteDto? AssignedVehicle { get; set; }

        public string? AssignedDriverId { get; set; }
        public ApplicationUser? AssignedDriver { get; set; }
    }
}