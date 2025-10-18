using CarTransportDashboard.Models.Dtos.TransportJob;

namespace CarTransportDashboard.Models.Dtos.Vehicle
{
    public record VehicleWriteDto
    {
        public Guid Id { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public FuelType FuelType { get; set; }

        // Navigation
        public ICollection<TransportJobUpdateDto>? AssignedJobs { get; set; }
    }
}