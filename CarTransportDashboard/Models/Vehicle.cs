namespace CarTransportDashboard.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;

        // Navigation
        public ICollection<TransportJob>? AssignedJobs { get; set; }
    }
}
