using CarTransportDashboard.Models.Dtos.TransportJob;

namespace CarTransportDashboard.Models.Dtos.Vehicle
{
    public record VehicleReadDto
    {
        public Guid Id { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;

        // Navigation
        public ICollection<TransportJobReadDto>? AssignedJobs { get; set; }
    }
}