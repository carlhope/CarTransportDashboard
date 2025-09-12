using CarTransportDashboard.Models.Dtos.Vehicle;

namespace CarTransportDashboard.Models
{
    public class Vehicle
    {
        public Guid Id { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;

        // Navigation
        public ICollection<TransportJob>? AssignedJobs { get; set; }

        public Vehicle()
        {
            
        }

        public Vehicle(VehicleWriteDto dto)
        {
            Make = dto.Make;
            Model = dto.Model;
            RegistrationNumber = dto.RegistrationNumber;
            //AssignedJobs = dto.AssignedJobs;
        }
    }
}
