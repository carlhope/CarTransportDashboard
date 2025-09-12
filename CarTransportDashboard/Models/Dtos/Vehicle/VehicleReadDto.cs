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

        public VehicleReadDto()
        {
            
        }

        public VehicleReadDto(Models.Vehicle vehicle)
        {
            Make = vehicle.Make;
            Model = vehicle.Model;
            RegistrationNumber = vehicle.RegistrationNumber;
        }
    }
}